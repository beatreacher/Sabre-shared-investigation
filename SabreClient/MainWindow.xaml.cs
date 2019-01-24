using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Autofac.Extras.NLog;
using Domain.Models;
using Newtonsoft.Json;
using SabreApiClient;
using SabreClientTest;

using BFM = SabreApiClient.BargainFinderMax;
using System.ComponentModel;
using System.Windows.Data;

namespace SabreClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ILogger _logger = new LoggerAdapter(NLog.LogManager.GetCurrentClassLogger());
        SessionManager _sessionManager;

        Dictionary<string, BFM.AirTripType> AirTripTypes = new Dictionary<string, BFM.AirTripType>
            {
                { "OneWay", BFM.AirTripType.OneWay },
                { "Return", BFM.AirTripType.Return },
                { "Circle", BFM.AirTripType.Circle },
                { "OpenJaw", BFM.AirTripType.OpenJaw },
                { "Other", BFM.AirTripType.Other }
            };

        IList<FlightDescription> BfmFlightDescriptions = new List<FlightDescription>();
        IList<FlightDescription> EnhancedFlightDescriptions = new List<FlightDescription>();
        BFM.PricedItineraryType[] Itineraries = null;

        private BFM.PricedItineraryType _minItinerary;
        private decimal _minAmount;
        private PNRTests _pnrTests = new PNRTests();

        private JsonSerializerSettings JSS = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private static readonly Credentials ApiCredentials = new Credentials(
                ConfigurationManager.AppSettings["UserName"],
                ConfigurationManager.AppSettings["Password"],
                ConfigurationManager.AppSettings["Organization"],
                ConfigurationManager.AppSettings["Domain"]
                );

        private static Session CurrentSession;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task SessionInitialize()
        {
            if (_sessionManager == null)
            {
                _sessionManager = new SessionManager(_logger);
            }

            if (CurrentSession == null)
            {
                CurrentSession = await _sessionManager.CreateSession(SessionTests.ApiCredentials, "SessionCreateRQ");
            }
        }

        private async void btnSearchBFM_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;

                await SessionInitialize();

                var bfm = new BargainFinderMaxTests();
                var bfms = await bfm.GetBargainFinderMax
                (
                    CurrentSession,
                    BfmFlightDescriptions,
                    //new List<FlightDescription>
                    //{
                    //    new FlightDescription { OriginLocation = txtOriginLocation1.Text, DestinationLocation = txtDestinationLocation1.Text, DepartureDateTime = txtDeparture1.Text },
                    //    new FlightDescription { OriginLocation = txtOriginLocation2.Text, DestinationLocation = txtDestinationLocation2.Text, DepartureDateTime = txtDeparture2.Text }
                    //},
                    txtItemsCount.Text,
                    AirTripTypes[cmbTripType.Text]
                );

                var bfmAirLowFareSearchRS = JsonConvert.SerializeObject(bfms.OTA_AirLowFareSearchRS, Formatting.Indented, JSS);
                File.WriteAllText("bfm.txt", bfmAirLowFareSearchRS);
                Process.Start("bfm.txt");
                //txtResponse.Text = bfmAirLowFareSearchRS;

                var itineraries = (BFM.OTA_AirLowFareSearchRSPricedItineraries)bfms.OTA_AirLowFareSearchRS.Items.FirstOrDefault(i => i is BFM.OTA_AirLowFareSearchRSPricedItineraries);
                var airOrders = (BFM.OTA_AirLowFareSearchRSTPA_Extensions)bfms.OTA_AirLowFareSearchRS.Items.FirstOrDefault(i => i is BFM.OTA_AirLowFareSearchRSTPA_Extensions);
                
                if (itineraries == null)
                {
                    return;
                }
                Itineraries = itineraries.PricedItinerary;

                GetMinItinerary(itineraries.PricedItinerary);
                ShowCheapestFlyInfo();

                var aloi = airOrders.AirlineOrderList[0];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private async void btnCreatePnr_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;
                await CreatePnr();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private async Task CreatePnr()
        {
            await SessionInitialize();

            var pnrResponse = await _pnrTests.CreatePNR(CurrentSession, null, txtOriginLocation1.Text, txtPnrDepartureDateTime.Text, txtAirlineCode.Text);
            var resp = JsonConvert.SerializeObject(pnrResponse.PassengerDetailsRS, Formatting.Indented, JSS);
            //txtPnrCreateResponse.Text = resp;

            File.WriteAllText("newPnr.txt", resp);
            Process.Start("newPnr.txt");

            txtPNR.Text = pnrResponse.PassengerDetailsRS.ItineraryRef.ID;
        }

        private async void btnEnhanced_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;

                await Enhance();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private async Task Enhance()
        {
            await SessionInitialize();

            var enhancedAirBookTests = new EnhancedAirBookTests();
            var enhResp = await enhancedAirBookTests.CreateEnhanced(
                CurrentSession, 
                txtPNR.Text,
                new List<FlightDescription>
                {
                    new FlightDescription
                    {
                        OriginLocation = txtOriginLocation1.Text,
                        DestinationLocation = txtDestinationLocation1.Text,
                        DepartureDateTime = txtDeparture1.Text,
                        MarketingAirline = txtAirlineCode.Text,
                        FlightNumber = txtFlightNumber.Text,
                        Status = "NN",
                        ResBookDesigCode = txtResBookDesigCode.Text,
                        NumberInParty = "1",
                        InstantPurchase = false
                    }
                });

            var resp = JsonConvert.SerializeObject(enhResp.EnhancedAirBookRS, Formatting.Indented, JSS);
            File.WriteAllText("enhanced.txt", resp);
            Process.Start("enhanced.txt");

            var endTransactionResp = await _pnrTests.EndTransaction(CurrentSession);
            resp = JsonConvert.SerializeObject(endTransactionResp.EndTransactionRS, Formatting.Indented, JSS);
            File.WriteAllText("endtransaction.txt", resp);
            Process.Start("endtransaction.txt");

            //txtEnhancedResponse.Text = resp;
        }

        private async void btnUpdatePnr_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;

                await UpdatePnr();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private async Task UpdatePnr()
        {
            await SessionInitialize();
            var updatedPnrResponse = await _pnrTests.CreatePNR(CurrentSession, txtPNR.Text, txtOriginLocation1.Text, txtPnrDepartureDateTime.Text, txtResBookDesigCode.Text);
            var resp = JsonConvert.SerializeObject(updatedPnrResponse.PassengerDetailsRS, Formatting.Indented, JSS);

            File.WriteAllText("updatedPnr.txt", resp);
            Process.Start("updatedPnr.txt");
            //txtPnrUpdateResponse.Text = resp;
        }

        private async void btnLoadPnr_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                await SessionInitialize();
                this.Cursor = Cursors.Wait;

                var loadPnrResp = await _pnrTests.LoadPNR(CurrentSession, txtPNR.Text);
                var resp = JsonConvert.SerializeObject(loadPnrResp.TravelItineraryReadRS, Formatting.Indented, JSS);

                File.WriteAllText("loadedPnr.txt", resp);
                Process.Start("loadedPnr.txt");
                //txtPnrLoadResponse.Text = resp;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private async void btnCheapest_Click(object sender, RoutedEventArgs ev)
        {
            var cursor = this.Cursor;
            try
            {
                this.Cursor = Cursors.Wait;

                var fs = _minItinerary.AirItinerary.OriginDestinationOptions[0].FlightSegment[0];

                //txtOriginLocation1.Text = fs.DepartureAirport.LocationCode;
                //txtDestinationLocation1.Text = fs.ArrivalAirport.LocationCode;
            
                txtResBookDesigCode.Text = txtCheapResBookDesigCode.Text;
                txtFlightNumber.Text = txtCheapFlightNumber.Text;
                txtAirlineCode.Text = txtCheapAirlineCode.Text;
                txtPnrDepartureDateTime.Text = DateTime.Parse(fs.DepartureDateTime).ToString("MM-dd");
                txtDeparture1.Text = txtCheapDeparture.Text;

                await CreatePnr();
                await Enhance();
                await UpdatePnr();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                this.Cursor = cursor;
            }
        }

        private void GetMinItinerary(BFM.PricedItineraryType[] itineraries)
        {
            _minItinerary = itineraries[0];
            _minAmount = _minItinerary.AirItineraryPricingInfo[0].ItinTotalFare.TotalFare.Amount;
            foreach (var itinerary in itineraries)
            {
                foreach (var pricing in itinerary.AirItineraryPricingInfo)
                {
                    if (pricing.ItinTotalFare.TotalFare.Amount < _minAmount)
                    {
                        _minAmount = pricing.ItinTotalFare.TotalFare.Amount;
                        _minItinerary = itinerary;
                    }
                }
            }
        }

        private void ShowCheapestFlyInfo()
        {
            txtSegmentsCount.Text = _minItinerary.AirItinerary.OriginDestinationOptions[0].FlightSegment.Length.ToString();
            txtPrice.Text = _minAmount.ToString();

            var fs = _minItinerary.AirItinerary.OriginDestinationOptions[0].FlightSegment[0];
            txtCheapResBookDesigCode.Text = fs.ResBookDesigCode;
            txtCheapFlightNumber.Text = fs.OperatingAirline.FlightNumber;
            txtCheapAirlineCode.Text = !string.IsNullOrWhiteSpace(fs.OperatingAirline.Code) ? fs.OperatingAirline.Code : fs.MarketingAirline.Code;
            txtCheapDeparture.Text = fs.DepartureDateTime;

            var minItinerarySerialized = JsonConvert.SerializeObject(_minItinerary, Formatting.Indented, JSS);
            File.WriteAllText("minItinerarySerialized.txt", minItinerarySerialized);
            Process.Start("minItinerarySerialized.txt");
        }

        private void btnAddBfmDestinationPoint_Click(object sender, RoutedEventArgs e)
        {
            BfmFlightDescriptions.Add(new FlightDescription { OriginLocation = txtOriginLocation1.Text, DestinationLocation = txtDestinationLocation1.Text, DepartureDateTime = txtDeparture1.Text });

            lvBfmFlightDescriptions.ItemsSource = BfmFlightDescriptions;
            ICollectionView view = CollectionViewSource.GetDefaultView(lvBfmFlightDescriptions.ItemsSource);
            view.Refresh();
        }

        private void btnClearBfmDestinationPoint_Click(object sender, RoutedEventArgs e)
        {
            BfmFlightDescriptions.Clear();

            lvBfmFlightDescriptions.ItemsSource = BfmFlightDescriptions;
            ICollectionView view = CollectionViewSource.GetDefaultView(lvBfmFlightDescriptions.ItemsSource);
            view.Refresh();
        }
    }
}