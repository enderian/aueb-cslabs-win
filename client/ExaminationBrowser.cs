using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cslabs_win.client
{
    public partial class ExaminationBrowser : Form
    {
        public static ChromiumWebBrowser ChromeBrowser;
        private static ExaminationMode _examination;

        public ExaminationBrowser(string username, string fullName, ExaminationMode examination)
        {
            _examination = examination;
            InitializeComponent();

            usernameLabel.Text = username;
            fullNameLabel.Text = fullName;
            machineNameLabel.Text = Environment.MachineName;

            var settings = new CefSettings
            {
                IgnoreCertificateErrors = true
            };
            Cef.Initialize(settings);

            ChromeBrowser = new ChromiumWebBrowser(_examination.BrowserURL)
            {
                MenuHandler = new MenuHandler(),
                RequestHandler = new RequestHandler(),
                LifeSpanHandler = new LifeSpanHandler()
            };
        }

        private void ExaminationBrowser_Load(object sender, EventArgs e)
        {
            _examination.Start();
            browserPanel.Controls.Add(ChromeBrowser);
            ChromeBrowser.Dock = DockStyle.Fill;
        }

        internal class MenuHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {
               
            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }

        internal class DownloadHandler : IDownloadHandler
        {
            public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
            {
                MessageBox.Show("Downloading files is not permitted.\nContact an administrator if you think this is incorrect.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                downloadItem.IsCancelled = true;
            }

            public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
            {
                throw new NotImplementedException();
            }
        }

        internal class LifeSpanHandler : ILifeSpanHandler
        {
            public bool DoClose(IWebBrowser browserControl, IBrowser browser)
            {
                return false;
            }

            public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
            {
               
            }

            public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
            {
                
            }

            public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
            {
                if (_examination.BrowserPopups)
                {
                    newBrowser = null;
                    return false;
                }
                else
                {
                    MessageBox.Show("Opening external links is not permitted.\nContact an administrator if you think this is incorrect.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    newBrowser = null;
                    return true;
                }
            }
        }

        internal class RequestHandler : IRequestHandler
        {
            public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
            {
                return false;
            }

            public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return null;
            }

            public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
            {
                var targetUri = new Uri(request.Url);
                if (_examination.BrowserAllowedDomains == null ||
                    _examination.BrowserAllowedDomains.Contains(targetUri.Host)) return false;
                MessageBox.Show(@"Visiting " + targetUri.Host + " is not permitted.\nContact an administrator if you think this is incorrect.", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }

            public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            {
                return CefReturnValue.Continue;
            }

            public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
            {
                return false;
            }

            public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
            {
                return false;
            }

            public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
            {
                
            }

            public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
            {
                return false;
            }

            public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
            {
                return false;
            }

            public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
            {
                
            }

            public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
            {
                
            }

            public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
            {
                
            }

            public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
            {
               
            }

            public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return false;
            }

            public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
            {
                return false;
            }
        }

        private void ExaminationBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = e.CloseReason != CloseReason.WindowsShutDown && e.CloseReason != CloseReason.ApplicationExitCall;
        }

        public void logoutButton_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown", "/l /f");
            Application.Exit();
        }
    }
}
