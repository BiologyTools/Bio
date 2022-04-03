![alt text](https://github.com/RepoErik/BioImage/blob/master/banner.bmp?raw=true)

# BioImage Library

A .NET Library for opening & annotating various microscopy imaging formats. Supports all bioformats supported images like .TIF, .CZI etc. 
Best for working with ROI's in OME format & CSV.

## Features

- Exporting ROI's to CSV files for easy analysis in statistics programs.

- Great at viewing image stacks with scroll wheel moving Z-plane and mouse side buttons scrolling C-planes.

- RGB image viewing mode which automatically combines 3 channels into RGB image & shows ROI from each channel which can be configured in ROI Manager.

- Editing & saving ROI's in images to OME format image stacks.

- Exporting ROI's from each image in a folder of images to CSV. For further quick & easy analysis of images.

- Exports bounding boxes of ROI's in CSV format.

- Select multiple points by holding down left control key for move & delete tools.

## Dependencies
-	[BioFormats.Net](https://github.com/GDanovski/BioFormats.Net)
-	[IKVM](http://www.ikvm.net/)
-	[AForge](http://www.aforgenet.com/)

## License
- BioImage [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- AForge [LGPL](http://www.aforgenet.com/framework/license.html)
- BioFormats.Net [GPL3](https://www.gnu.org/licenses/gpl-3.0.en.html)
- [IKVM](https://github.com/gluck/ikvm/blob/master/LICENSE)

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
		//ushort[,] GetBlock(int ix, int iy, int iw, int ih)
		ushort[,] val = b.GetBlock(0, 0, 0, 0, 0, 0, 150, 150);
		//SetBlock(int ix, int iy, int iw, int ih, ushort[,] sh)
            	b.SetBlock(0, 0, 0, 0, 150, 150, 150, 150, val);
		b.SaveSeries("E://TESTIMAGES//save.ome.tif",0);
		bio.SaveSeries("E://TESTIMAGES//subStack.ome.tif",0);
		ImageViewer iv = new ImageViewer(bio);
		//We open the result in an ImageViewer.
		iv.ShowDialog();
		return val.ToString();
	}
}




