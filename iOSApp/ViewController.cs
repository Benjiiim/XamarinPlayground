using System;
using System.IO;
using Foundation;
using UIKit;

namespace iOSApp
{
	public partial class ViewController : UIViewController
	{
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		partial void OnTakePhotoPressed (UIButton sender)
		{
			TakePhotoButton.Enabled = false;

			UIImagePickerController picker = new UIImagePickerController ();
			picker.SourceType = UIImagePickerControllerSourceType.Camera;

			picker.FinishedPickingMedia += async (o, e) => {
				// Create a moderate quality version of the image
				byte [] dataBytes;
				using (NSData data = e.OriginalImage.AsJPEG (.5f)) {
					dataBytes = new byte [data.Length];
					Console.WriteLine (data.Length);
					System.Runtime.InteropServices.Marshal.Copy (data.Bytes, dataBytes, 0, Convert.ToInt32 (data.Length));
				}

				ThePhoto.Image = e.OriginalImage;
				DetailsText.Text = "Processing...";

				((UIImagePickerController)o).DismissViewController (true, null);

				// Create a stream, send it to MCS, and get back 
				using (MemoryStream stream = new MemoryStream (dataBytes)) {
					float happyValue = await SharedProject.Core.GetAverageHappinessScore (stream);
					DetailsText.Text = SharedProject.Core.GetHappinessMessage (happyValue);
					TakePhotoButton.Enabled = true;

				}
			};
			PresentModalViewController (picker, true);
		}
	}
}

