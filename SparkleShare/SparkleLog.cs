//   SparkleShare, an instant update workflow to Git.
//   Copyright (C) 2010  Hylke Bons <hylkebons@gmail.com>
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program. If not, see <http://www.gnu.org/licenses/>.

using Gtk;
using Mono.Unix;
using SparkleLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
//using WebKit;

namespace SparkleShare {

	public class SparkleLog : Window {

		public readonly string LocalPath;
		private VBox LayoutVertical;
		private ScrolledWindow ScrolledWindow;
		private MenuBar MenuBar;
//		private WebView WebView;

		// Short alias for the translations
		public static string _ (string s)
		{
			return Catalog.GetString (s);
		}

		

		public SparkleLog (string path) : base ("")
		{

			LocalPath = path;
			
			string name = System.IO.Path.GetFileName (LocalPath);
			SetSizeRequest (480, 640);

	 		SetPosition (WindowPosition.Center);
			BorderWidth = 0;
			
			// TRANSLATORS: {0} is a folder name, and {1} is a server address
			Title = String.Format(_("Recent Events in ‘{0}’"), name);
			IconName = "folder-sparkleshare";

			DeleteEvent += delegate {
				Close ();
			};
			
			LayoutVertical = new VBox (false, 0);

			LayoutVertical.PackStart (CreateEventLog (), true, true, 0);

				HButtonBox dialog_buttons = new HButtonBox {
					Layout = ButtonBoxStyle.Edge,
					BorderWidth = 12
				};

					Button open_folder_button = new Button (_("_Open Folder")) {
						UseUnderline = true
					};
 
					open_folder_button.Clicked += delegate (object o, EventArgs args) {

						Process process = new Process ();
						process.StartInfo.FileName  = Defines.OPEN_COMMAND;
						process.StartInfo.Arguments = LocalPath.Replace (" ", "\\ "); // Escape space-characters
						process.Start ();

						Close ();

					};

					Button close_button = new Button (Stock.Close);

					close_button.Clicked += delegate {
						Close ();
					};

				dialog_buttons.Add (open_folder_button);
				dialog_buttons.Add (close_button);

			LayoutVertical.PackStart (new HSeparator (), false, false, 0);

			// We have to hide the menubar somewhere...
			LayoutVertical.PackStart (EnableKeyboardShortcuts (), false, false, 0);
			LayoutVertical.PackStart (dialog_buttons, false, false, 0);

			Add (LayoutVertical);

		}


		private MenuBar EnableKeyboardShortcuts ()
		{

			// Adds a hidden menubar that to enable keyboard
			// shortcuts to close the log
			MenuBar = new MenuBar ();

				MenuItem file_item = new MenuItem ("File");

				    Menu file_menu = new Menu ();

						MenuItem close_1 = new MenuItem ("Close1");
						MenuItem close_2 = new MenuItem ("Close2");
		
						// adds specific Ctrl+W and Esc key accelerators to Log Window
						AccelGroup accel_group = new AccelGroup ();
						AddAccelGroup (accel_group);

						// Close on Esc
						close_1.AddAccelerator ("activate", accel_group, new AccelKey (Gdk.Key.W, Gdk.ModifierType.ControlMask,
							AccelFlags.Visible));

						close_1.Activated += delegate { Close (); };

						// Close on Ctrl+W
						close_2.AddAccelerator ("activate", accel_group, new AccelKey (Gdk.Key.Escape, Gdk.ModifierType.None,
							AccelFlags.Visible));
						close_2.Activated += delegate { Close (); };

					file_menu.Append (close_1);
					file_menu.Append (close_2);

				file_item.Submenu = file_menu;

			MenuBar.Append (file_item);

			// Hacky way to hide the menubar, but the accellerators
			// will simply be disabled when using Hide ()
			MenuBar.HeightRequest = 1;
			MenuBar.ModifyBg (StateType.Normal, Style.Background (StateType.Normal));

			return MenuBar;

		}


		public void Close ()
		{

			foreach (SparkleRepo repo in SparkleShare.Controller.Repositories) {

				if (repo.LocalPath.Equals (LocalPath)) {

/*				// Remove the eventhooks
					repo.NewCommit -= UpdateEventLog;
					repo.PushingFinished -= UpdateEventLog;
					repo.PushingFailed -= UpdateEventLog;    TODO: Move to controller
					repo.FetchingFinished -= UpdateEventLog;
					repo.FetchingFailed -= UpdateEventLog;*/

				}

			}

			HideAll ();

		}


		public void UpdateEventLog ()
		{

			Application.Invoke (delegate {

				LayoutVertical.Remove (ScrolledWindow);
				ScrolledWindow = CreateEventLog ();
				LayoutVertical.PackStart (ScrolledWindow, true, true, 0);
				LayoutVertical.ReorderChild (ScrolledWindow, 0);
				ShowAll ();

			});

		}


		private ScrolledWindow CreateEventLog ()
		{

/*			List <ChangeSet> changesets = new List <ChangeSet> ();

			foreach (SparkleRepo repo in SparkleShare.Controller.Repositories) {
// Controller.GetCommits (LocalPath);
				// Get commits from the repository
				if (repo.LocalPath.Equals (LocalPath)) {

					 changesets = repo.GetCommits (25);

/*					// Update the log when there are new remote changes
					repo.NewCommit += UpdateEventLog;

					// Update the log when changes are being sent
					repo.PushingFinished += UpdateEventLog;
					repo.PushingFailed += UpdateEventLog;

					repo.FetchingFinished += UpdateEventLog;   TODO: Move to controller
					repo.FetchingFailed += UpdateEventLog;

					break;

				}

			}*/


			

			// Style the html page like the GTK theme
		/*	html = html.Replace ("<!-- $body-font-size -->", (Style.FontDescription.Size / 1024 + 0.5) + "pt");
			html = html.Replace ("<!-- $body-font-family -->", "\"" + Style.FontDescription.Family + "\"");
			html = html.Replace ("<!-- $body-color -->", SparkleUIHelpers.GdkColorToHex (Style.Foreground (StateType.Normal)));
			html = html.Replace ("<!-- $body-background-color -->", SparkleUIHelpers.GdkColorToHex (new TreeView ().Style.Base (StateType.Normal)));


			html = html.Replace ("<!-- $day-entry-header-background-color -->", SparkleUIHelpers.GdkColorToHex (Style.Background (StateType.Normal)));
			html = html.Replace ("<!-- $secondary-font-color -->", SparkleUIHelpers.GdkColorToHex (Style.Foreground (StateType.Insensitive)));

Console.WriteLine (Style.FontDescription.Family);

			WebView = new WebView () {
				Editable = false
			};
			
			
			 WebView.HoveringOverLink += delegate (object o, WebKit.HoveringOverLinkArgs args) {
        Status = args.Link;
        };
WebView.NavigationRequested += delegate (object o, WebKit.NavigationRequestedArgs args) {
        // FIXME: There's currently no way to tell the difference 
        // between a link being clicked and another navigation event.
        // This is a temporary workaround.
        Console.WriteLine ("CLICKED!" + args.Request.Uri);
        
        if (args.Request.Uri == Status) {
        
Console.WriteLine ("CLICKED!:" + Status);
            }
            };



//				wrapper.ModifyBg (StateType.Normal, background_color);

				WebView.LoadHtmlString (html, "file://");
				WebView.HoveringOverLink += delegate {};
			 */

			ScrolledWindow = new ScrolledWindow () {
				HscrollbarPolicy = PolicyType.Never
			};
			
			ScrolledWindow.AddWithViewport (new Label (SparkleShare.Controller.GetHTMLLog (LocalPath)));
			(ScrolledWindow.Child as Viewport).ShadowType = ShadowType.None;

			return ScrolledWindow;

		}
public string Status = null;

	}



}
