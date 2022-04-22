![alt text](https://github.com/RepoErik/BioImage/blob/master/banner.bmp?raw=true)

# BioImage Library

A .NET Library for opening & annotating various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. 
Best for working with ROI's in OME format & CSV.

## Features

- Exporting ROI's to CSV files for easy analysis in statistics programs.

- Viewing image stacks with scroll wheel moving Z-plane and mouse side buttons scrolling C-planes.

- RGB image viewing mode which automatically combines 3 channels into RGB image & shows ROI from each channel which can be configured in ROI Manager.

- Editing & saving ROI's in images to OME format image stacks.

- Exporting ROI's from each OME image in a folder of images to CSV.

- Select multiple points by holding down left control key for move & delete tools. Delete button for delete.
- Open & Save ImageJ Tiff files and embed ROI's in image Description tag.
- C# Scripting with sample tool script and regular scripts in "/Scripts/" folder.

## Dependencies
- [BioFormats.Net](https://github.com/GDanovski/BioFormats.Net)
- [IKVM](http://www.ikvm.net/)
- [AForge](http://www.aforgenet.com/)
- [LibTiff.Net](https://bitmiracle.com/libtiff/)
- [Cs-script](https://github.com/oleg-shilo/cs-script/blob/master/LICENSE)

## Licenses
- BioImage [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- AForge [LGPL](http://www.aforgenet.com/framework/license.html)
- BioFormats.Net [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- [IKVM](https://github.com/gluck/ikvm/blob/master/LICENSE)
- LibTiff.Net [BSD](https://bitmiracle.com/libtiff/)
- Cs-script [MIT](https://github.com/oleg-shilo/cs-script/blob/master/LICENSE)

## Example usage.

ImageView imageview = new ImageView("16bitTestStack.ome.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

//Another way of opening just image.

BioImage image = new BioImage(0,"16bitTestStack.ome.tif");

//Get RGB Bitmap of BioImage with coordinates (Series, Z-depth, Channel, Time)

Bitmap rgb = image.GetImageRGB(0,0,0,0);

//Get Filtered Bitmap of BioImage with coordinates (Series, Z-depth, Channel, Time)

Bitmap filt = image.GetImageFiltered(0,0,0,0);

image.SaveSeries("16bitTestSaveStack.ome.tif", 0);

## Scripting
-  Save scripts into "StartupPath/Scripts" with ".cs" ending. Open script editor & script runner from menu
-  Double click on script name in Script runner to run script.
-  Scripts saved in Scripts folder will be loaded into script runner.
-  Program installer include sample script "Sample.cs" which gets & sets pixels and saves resulting image.
-  Use Script recorder to record program function calls and script runner to turn recorder text into working scripts. (See sample [scripts](https://github.com/BioMicroscopy/BioImage-Scripts))
## Sample Script

//css_reference BioImage.dll;

using System;

using System.Windows.Forms;

using BioImage;

public class Loader
{

	public string Load()
	{	
		BioImage.BioImage b =  new BioImage.BioImage("E://TESTIMAGES//text.ome.tif",0);
		//We create a substack of BioImage b.
		BioImage.BioImage bio = new BioImage.BioImage(b,"subStack.ome.tif", 0, 0, 3, 0, 3, 0, 2);
		//SetValueRGB(int s, int z, int c, int t, int x, int y, int RGBindex, ushort value)
		b.SetValueRGB(0,0,0,0,0,0,0,15000);
		//GetValueRGB(int s, int z, int c, int t, int x, int y, int RGBindex);
		ushort val = b.GetValueRGB(0,0,0,0,0,0,1);
		b.SaveSeries("E://TESTIMAGES//save.ome.tif",0);
		bio.SaveSeries("E://TESTIMAGES//subStack.ome.tif",0);
		ImageViewer iv = new ImageViewer(bio);
		//We open the result in an ImageViewer.
		iv.ShowDialog();
		return val.ToString();
	}
	
}
## Sample Tool Script

//css_reference BioImage.dll;

using System;

using System.Windows.Forms;

using System.Drawing;

using BioImage;

public class Loader
{

	public string Load()
	{
		do
		{
			BioImage.Scripting.State s = BioImage.Scripting.GetState();
			if (s != null)
			{
				if (s.p.X < 10 && s.p.Y < 10)
				{
					return "Corner (" + s.p.X + ", " + s.p.Y + ")";

				}
				if (s.type == BioImage.Scripting.Event.Move)
				{
					string st = "Move (" + s.p.X + ", " + s.p.Y + ")";
					if (s.p.X < 25 && s.p.Y < 25)
					{
						return st;
					}
					BioImage.Scripting.LogLine(st);
				}
				else
				if (s.type == BioImage.Scripting.Event.Up)
				{
					string st = "Up (" + s.p.X + ", " + s.p.Y + ")";
					if (s.p.X < 50 && s.p.Y < 50)
					{
						return st;
					}
					BioImage.Scripting.LogLine(st);
				}
				else
				if (s.type == BioImage.Scripting.Event.Down)
				{
					string st = "Down (" + s.p.X + ", " + s.p.Y + ")";
					if (s.p.X < 75 && s.p.Y < 75)
					{
						return st;
					}
					BioImage.Scripting.LogLine(st);
				}
			}
		} while (true);

		return "Done";
	}
}




