BioImage

.NET Library for opening various microscopy imaging formats. Supports all bioformats supported images. Like .CZI, .TIF. Includes tools for changing channel minimum & maximum values. Tool for changing range of Z-Slices & Timeline, as well as changing timer speed for playback of slices & timeframes.

Example usage.

ImageView imageview = new ImageView("16bitTestStack.tif");

imageview.Dock = DockStyle.Fill;

mainTabControl.TabPages[3].Controls.Add(imageview);

