using System;
using System.Collections.Generic;
using System.Text;

namespace TawalaTest.TestingSupport
{
	public class ImageTestBase
	{
		protected static string oneWhitePixelJpgFileName = "OneWhitePixel.jpg";
		protected static string oneBlackPixelJpgFileName = "OneBlackPixel.jpg";
		protected static string unpronounceableJpgFileName = "{} []^`~.jpg";

		protected static string oneWhitePixelJpgPath
		{
			get { return Util.GetTestFilePath(oneWhitePixelJpgFileName); }
		}

		protected static string oneBlackPixelJpgPath
		{
			get { return Util.GetTestFilePath(oneBlackPixelJpgFileName); }
		}

		protected static string unpronounceableJpgPath
		{
			get { return Util.GetTestFilePath(unpronounceableJpgFileName); }
		}

		protected static string oneWhitePixelJpgUrl
		{
			get { return toUrl(oneWhitePixelJpgPath); }
		}

		protected static string oneBlackPixelJpgUrl
		{
			get { return toUrl(oneBlackPixelJpgPath); }
		}

		protected static string unpronounceableJpgUrl
		{
			get { return toUrl(unpronounceableJpgPath); }
		}

		private static string toUrl(string pathName)
		{
			return "file:///" + urlEncode(pathName).Replace("\\", "/");
		}

		private static string urlEncode(string decodedString)
		{
			string encodedString = decodedString;

			encodedString = encodedString.Replace(" ", "%20");
			encodedString = encodedString.Replace("[", "%5B");
			encodedString = encodedString.Replace("]", "%5D");
			encodedString = encodedString.Replace("^", "%5E");
			encodedString = encodedString.Replace("`", "%60");
			encodedString = encodedString.Replace("{", "%7B");
			encodedString = encodedString.Replace("}", "%7D");
			encodedString = encodedString.Replace("~", "%7E");

			return encodedString;
		}

		protected static string oneWhitePixelJpgDataString =
			@"/9j/4AAQSkZJRgABAgEAYABgAAD/wAARCAABAAEDAREAAhEBAxEB/9sAhAABAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBAQEBA" +
			@"QEBAQICAgECAgIBAQIDAgICAgMDAwECAwMDAgMCAgMCAQEBAQEBAQEBAQECAQEBAQICAgICAgICAgICAgICAgICAgICAgICAgICAg" +
			@"ICAgICAgICAgICAgICAgICAgICAgL/xAGiAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJCgsQAAIBAwMCBAMFBQQEAAABfQECAwA" +
			@"EEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hp" +
			@"anN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9" +
			@"PX29/j5+gEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoLEQACAQIEBAMEBwUEBAABAncAAQIDEQQFITEGEkFRB2FxEyIygQgUQp" +
			@"GhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqCg4SFhoeIiYq" +
			@"Sk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQA/" +
			@"AP7+KAP/2Q==";

		protected static string oneWhitePixelPngDataString =
			@"iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAALHRFWHRDcmVhdGlvbiBUaW1lAEZyaSAyNyBKdW4gMjAwOCAxNzoxN" +
			@"DoxMiAtMDgwMM9P8akAAAAHdElNRQfYBhwADiF13BS1AAAACXBIWXMAAA7DAAAOwwHHb6hkAAAABGdBTUEAALGPC/xhBQAAAAtJRE" +
			@"FUeNpjYAACAAAFAAHp+tzYAAAAAElFTkSuQmCC";

		protected static string oneWhitePixelGifDataString =
			@"R0lGODlhAQABAIcAAP///wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
			@"AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAAAAP8ALAAAAAABAAEAAAgEAAEEBAA7";

	}
}
