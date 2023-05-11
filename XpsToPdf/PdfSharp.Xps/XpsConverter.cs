using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PdfSharp.Xps.XpsModel;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Xps.Rendering;
using FixedDocument = PdfSharp.Xps.XpsModel.FixedDocument;
using FixedPage = PdfSharp.Xps.XpsModel.FixedPage;
using IOPath = System.IO.Path;

namespace PdfSharp.Xps
{
  /// <summary>
  /// Main class that provides the functionallity to convert an XPS file into a PDF file.
  /// </summary>
  public class XpsConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="XpsConverter"/> class.
    /// </summary>
    /// <param name="pdfDocument">The PDF document.</param>
    /// <param name="xpsDocument">The XPS document.</param>
    public XpsConverter(PdfDocument pdfDocument, XpsDocument xpsDocument)
    {
      if (pdfDocument == null)
        throw new ArgumentNullException("pdfDocument");
      if (xpsDocument == null)
        throw new ArgumentNullException("xpsDocument");

      this.pdfDocument = pdfDocument;
      this.xpsDocument = xpsDocument;

      Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XpsConverter"/> class.
    /// </summary>
    /// <param name="pdfDocument">The PDF document.</param>
    /// <param name="xpsDocumentPath">The XPS document path.</param>
    public XpsConverter(PdfDocument pdfDocument, string xpsDocumentPath)  // TODO: a constructor with an Uri
    {
      if (pdfDocument == null)
        throw new ArgumentNullException("pdfDocument");
      if (String.IsNullOrEmpty(xpsDocumentPath))
        throw new ArgumentNullException("xpsDocumentPath");

      this.pdfDocument = pdfDocument;
      xpsDocument = XpsDocument.Open(xpsDocumentPath);

      Initialize();
    }

    void Initialize()
    {
      context = new DocumentRenderingContext(pdfDocument);
    }

    DocumentRenderingContext Context => context;
    DocumentRenderingContext context;

    /// <summary>
    /// Gets the PDF document of this converter.
    /// </summary>
    public PdfDocument PdfDocument => pdfDocument;

    PdfDocument pdfDocument;

    /// <summary>
    /// Gets the XPS document of this converter.
    /// </summary>
    public XpsDocument XpsDocument => xpsDocument;

    XpsDocument xpsDocument;

    //
    /// <summary>
    /// Implements the PDF file to XPS file conversion.
    /// </summary>
    public static void Convert(XpsDocument xpsDocument, string pdfFilename, int docIndex)
    {

        if (xpsDocument == null)
            throw new ArgumentNullException("xpsDocument");

        if (String.IsNullOrEmpty(pdfFilename))
            throw new ArgumentNullException("pdfFilename");

        PdfDocument pdfDocument = new PdfDocument();
        PdfRenderer renderer = new PdfRenderer();

        int pageIndex = 0;
        foreach (FixedDocument fixedDocument in xpsDocument.Documents)
        foreach (FixedPage page in fixedDocument.Pages)
        {
            if (page == null)
                continue;
            Debug.WriteLine(String.Format("  doc={0}, page={1}", docIndex, pageIndex));
            PdfPage pdfPage = renderer.CreatePage(pdfDocument, page);
            renderer.RenderPage(pdfPage, page);
            pageIndex++;
        }
        pdfDocument.Save(pdfFilename);

    }
        


    static public BitmapSource BitmapSourceFromPage(DocumentPage docPage, double resolution)
    {
      double pixelWidth = docPage.Size.Width * resolution / 96;
      double pixelHeight = docPage.Size.Height * resolution / 96;
      RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)pixelWidth, (int)pixelHeight, resolution, resolution, PixelFormats.Default);
      renderTarget.Render(docPage.Visual);

      return renderTarget;

      //PngBitmapEncoder encoder = new PngBitmapEncoder();  // Choose type here ie: JpegBitmapEncoder, etc   
      //encoder.Frames.Add(BitmapFrame.Create(renderTarget));

      //BitmapSource.Create(pageWidth, pageHeight, resolution, resolution, PixelFormats.)

      //return encoder.Preview;
      //encoder.
      //BitmapSource s = Xps;
      ////FileStream pageOutStream = new FileStream(xpsFileName + ".Page" + pageNum + ".bmp", FileMode.Create, FileAccess.Write);
      //MemoryStream memStream = new MemoryStream();
      //encoder.Save(memStream);
      //return memStream.ToArray();
    }

    //byte[] void PngFromPage(FixedDocument fixedDocument, int pageIndex, double resolution)
    //{
    //  if (fixedDocument==null)
    //    throw new ArgumentNullException("fixedDocument");
    //  if ( pageIndex<0|| pageIndex>= fixedDocument.PageCount)
    //    throw new ArgumentOutOfRangeException("pageIndex");

    //  FixedPage page = fixedDocument.Pages[pageIndex];
    //  double pageWidth = page.Width;
    //  double pageHeight= page.Height;

    //  // Create an appropirate render bitmap
    //  const int factor = 3;
    //  int width = (int)(WidthInPoint * factor);
    //  int height = (int)(HeightInPoint * factor);
    //  this.image = new RenderTargetBitmap(width, height, 72 * factor, 72 * factor, PixelFormats.Default);
    //  if (visual is UIElement)
    //  {
    //    // Perform layout on UIElement - otherwise nothing gets rendered
    //    UIElement element = visual as UIElement;
    //    Size size = new Size(WidthInPU, HeightInPU);
    //    element.Measure(size);
    //    element.Arrange(new Rect(new Point(), size));
    //    element.UpdateLayout();
    //  }
    //  this.image.Render(visual);

    //  // Save image as PNG
    //  FileStream stream = new FileStream(Path.Combine(OutputDirectory, Name + ".png"), FileMode.Create);
    //  PngBitmapEncoder encoder = new PngBitmapEncoder();
    //  //string author = encoder.CodecInfo.Author.ToString();
    //  encoder.Frames.Add(BitmapFrame.Create(this.image));
    //  encoder.Save(stream);
    //  stream.Close();
    //}
  }
}