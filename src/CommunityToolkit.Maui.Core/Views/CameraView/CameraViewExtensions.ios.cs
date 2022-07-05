using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVFoundation;
using CommunityToolkit.Maui.Core.Primitives;

namespace CommunityToolkit.Maui.Core.Views.CameraView;
public static class CameraViewExtensions
{
	public static AVCaptureFlashMode ToPlatform(this CameraFlashMode cameraFlashMode) => cameraFlashMode switch
	{
		CameraFlashMode.Off => AVCaptureFlashMode.Off,
		CameraFlashMode.On => AVCaptureFlashMode.On,
		CameraFlashMode.Auto => AVCaptureFlashMode.Auto,
		_ => throw new InvalidOperationException("The value doesn't exist."),
	};
}
