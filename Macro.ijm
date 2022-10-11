run("Bio-Formats Importer", "open=F:/TESTIMAGES/CZI/16Bit-ZStack.czi");
run("Remove Outliers...", "radius=2 threshold=50 which=Bright stack");
run("Bio-Formats Exporter", "save=F:/TESTIMAGES/CZI/16Bit-ZStack.ome.tif export compression=Uncompressed");
