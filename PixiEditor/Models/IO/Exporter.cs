﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PixiEditor.Models.DataHolders;
using PixiEditor.Models.Dialogs;

namespace PixiEditor.Models.IO
{
    public class Exporter
    {
        public static Size FileDimensions;
        public static string SaveDocumentPath { get; set; }

        public static void SaveAsNewEditableFile(Document document, bool updateWorkspacePath = false)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "PixiEditor Files | *.pixi",
                DefaultExt = "pixi"
            };
            if ((bool) dialog.ShowDialog()) SaveAsEditableFile(document, dialog.FileName, updateWorkspacePath);
        }

        public static void SaveAsEditableFile(Document document, string path, bool updateWorkspacePath = false)
        {
            BinarySerialization.WriteToBinaryFile(path, new SerializableDocument(document));

            if (updateWorkspacePath)
                SaveDocumentPath = path;
        }

        /// <summary>
        ///     Creates ExportFileDialog to get width, height and path of file.
        /// </summary>
        /// <param name="type">Type of file to be saved in.</param>
        /// <param name="bitmap">Bitmap to be saved as file.</param>
        public static void Export(WriteableBitmap bitmap, Size fileDimensions)
        {
            ExportFileDialog info = new ExportFileDialog(fileDimensions);
            //If OK on dialog has been clicked
            if (info.ShowDialog())
            {
                //If sizes are incorrect
                if (info.FileWidth < bitmap.Width || info.FileHeight < bitmap.Height)
                {
                    MessageBox.Show("Incorrect height or width value", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                FileDimensions = new Size(info.FileWidth, info.FileHeight);
                SaveAsPng(info.FilePath, info.FileHeight, info.FileWidth, bitmap);
            }
        }

        /// <summary>
        ///     Saves image to PNG file
        /// </summary>
        /// <param name="savePath">Save file path</param>
        /// <param name="exportWidth">File width</param>
        /// <param name="exportHeight">File height</param>
        private static void SaveAsPng(string savePath, int exportWidth, int exportHeight, WriteableBitmap bitmap)
        {
            try
            {
                bitmap = bitmap.Resize(exportWidth, exportHeight,
                    WriteableBitmapExtensions.Interpolation.NearestNeighbor);
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}