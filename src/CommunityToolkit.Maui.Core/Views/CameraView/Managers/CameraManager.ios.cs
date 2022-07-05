using AVFoundation;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Core.Views.CameraView;
using CoreFoundation;

namespace CommunityToolkit.Maui.Core;
public partial class CameraManager
{
	AVCaptureSession? captureSession;
	AVCaptureDevice? captureDevice;
	AVCaptureInput? captureInput = null;
	PreviewView? view;
	AVCaptureVideoDataOutput? videoDataOutput;
	AVCaptureVideoPreviewLayer? videoPreviewLayer;
	AVCapturePhotoOutput? photoOutput;
	//CaptureDelegate? captureDelegate;
	DispatchQueue? dispatchQueue;
	CameraFlashMode flashMode;
	public UIView CreateNativeView()
	{
		captureSession = new AVCaptureSession
		{
			SessionPreset = AVCaptureSession.Preset640x480
		};

		videoPreviewLayer = new(captureSession)
		{
			VideoGravity = AVLayerVideoGravity.ResizeAspectFill
		};
		view = new(videoPreviewLayer);

		return view;
	}


	protected virtual partial void PlatformConnect()
	{
		if (captureDevice is null || captureSession is null)
		{
			return;
		}

		// We need to use this method because the "StartRunning" call blocks the UI
		DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
		{
			if (captureSession.Running)
			{
				captureSession.StopRunning();
			}

			// Cleanup old input
			if (captureInput is not null && captureSession.Inputs.Length > 0 && captureSession.Inputs.Contains(captureInput))
			{
				captureSession.RemoveInput(captureInput);
				captureInput.Dispose();
				captureInput = null;
			}

			// Cleanup old device
			if (captureDevice is not null)
			{
				captureDevice.Dispose();
				captureDevice = null;
			}

			var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaTypes.Video.ToString());

			foreach (var device in devices)
			{
				if (cameraLocation == CameraLocation.Front && device.Position == AVCaptureDevicePosition.Front)
				{
					captureDevice = device;
					break;
				}
				else if (cameraLocation == CameraLocation.Rear && device.Position == AVCaptureDevicePosition.Back)
				{
					captureDevice = device;
					break;
				}
			}

			captureDevice ??= AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);
			if (captureDevice is null)
			{
				// Should we throw an Exception?
				return;
			}

			captureInput = new AVCaptureDeviceInput(captureDevice, out var err);

			captureSession.AddInput(captureInput);

			UpdateFlashMode(flashMode);

			photoOutput = new AVCapturePhotoOutput();

			if (captureSession.CanAddOutput(photoOutput))
			{
				captureSession.AddOutput(photoOutput);
				photoOutput.IsHighResolutionCaptureEnabled = true;
				photoOutput.IsLivePhotoCaptureEnabled = photoOutput.IsLivePhotoCaptureSupported;
			}

			captureSession.CommitConfiguration();
			captureSession.StartRunning();
		});
	}

	protected virtual partial void PlatformDisconnect() { }
	protected virtual partial void PlatformTakePicture() { }
	public partial void UpdateFlashMode(CameraFlashMode flashMode)
	{
		this.flashMode = flashMode;
		if (captureDevice is null)
		{
			return;
		}

		captureDevice.LockForConfiguration(out var err);
		
		captureDevice.FlashMode = flashMode.ToPlatform();
		
		captureDevice.UnlockForConfiguration();
	}
}

class PreviewView : UIView
{
	readonly AVCaptureVideoPreviewLayer previewLayer;

	public PreviewView(AVCaptureVideoPreviewLayer layer)
	{
		previewLayer = layer;

		previewLayer.Frame = Layer.Bounds;
		Layer.AddSublayer(previewLayer);
	}

	public override void LayoutSubviews()
	{
		base.LayoutSubviews();
		previewLayer.Frame = Layer.Bounds;
	}
}
