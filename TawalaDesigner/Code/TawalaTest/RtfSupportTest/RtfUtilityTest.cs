using System;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.RtfSupportTest
{
	[TestFixture]
	public class RtfUtilityTest
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void DecodeHexString()
		{
			Assert.AreEqual("Q1:a", RtfUtility.DecodeHexString("510031003a0061000000"));
			Assert.AreEqual("Q2", RtfUtility.DecodeHexString("510032000000"));
			Assert.AreEqual("Address", RtfUtility.DecodeHexString("41006400640072006500730073000000"));
			Assert.AreEqual("FirstName", RtfUtility.DecodeHexString("460069007200730074004e0061006d0065000000"));
			Assert.AreEqual("LastName", RtfUtility.DecodeHexString("4c006100730074004e0061006d0065000000"));
		}

		[Test]
		public void EncodeHexString()
		{
			Assert.AreEqual("510031003a0061000000", RtfUtility.EncodeHexString("Q1:a"));
			Assert.AreEqual("510032000000", RtfUtility.EncodeHexString("Q2"));
			Assert.AreEqual("41006400640072006500730073000000", RtfUtility.EncodeHexString("Address"));
			Assert.AreEqual("460069007200730074004e0061006d0065000000", RtfUtility.EncodeHexString("FirstName"));
			Assert.AreEqual("4c006100730074004e0061006d0065000000", RtfUtility.EncodeHexString("LastName"));
		}

		[Test]
		public void HexStringToBinary()
		{
			byte[] binary = RtfUtility.HexStringToBinary("FF7F3F");
			Assert.AreEqual(0xff, binary[0]);
			Assert.AreEqual(0x7f, binary[1]);
			Assert.AreEqual(0x3f, binary[2]);
		}

		[Test]
		public void WmfHeaderHexStringToBinary()
		{
			byte[] binary = RtfUtility.HexStringToBinary("0100090000033b0300000200050200000000");

			Assert.AreEqual(18, binary.Length);

			Assert.AreEqual(0x01, binary[0]);
			Assert.AreEqual(0x00, binary[1]);
			Assert.AreEqual(0x09, binary[2]);
			Assert.AreEqual(0x00, binary[3]);
			Assert.AreEqual(0x00, binary[4]);
			Assert.AreEqual(0x03, binary[5]);
			Assert.AreEqual(0x3b, binary[6]);
			Assert.AreEqual(0x03, binary[7]);
			Assert.AreEqual(0x00, binary[8]);
			Assert.AreEqual(0x00, binary[9]);
			Assert.AreEqual(0x02, binary[10]);
			Assert.AreEqual(0x00, binary[11]);
			Assert.AreEqual(0x05, binary[12]);
			Assert.AreEqual(0x02, binary[13]);
			Assert.AreEqual(0x00, binary[14]);
			Assert.AreEqual(0x00, binary[15]);
			Assert.AreEqual(0x00, binary[16]);
			Assert.AreEqual(0x00, binary[17]);
		}
		
		[Test]
		public void HexStringToUInt()
		{
			uint value = RtfUtility.HexStringToUInt("FF000000");
			Assert.AreEqual(0xff, value);

			value = RtfUtility.HexStringToUInt("7FFF0000");
			Assert.AreEqual(0xff7F, value);

			value = RtfUtility.HexStringToUInt("3F7FFF00");
			Assert.AreEqual(0xff7F3f, value);

			value = RtfUtility.HexStringToUInt("1F3F7FFF");
			Assert.AreEqual(0xff7F3f1f, value);
		}

		[Test]
		public void UIntToUpperCaseHexString()
		{
			string hexString = RtfUtility.UIntToUpperCaseHexString(0xff, 8);
			Assert.AreEqual("FF000000", hexString);

			hexString = RtfUtility.UIntToUpperCaseHexString(0xff7f, 8);
			Assert.AreEqual("7FFF0000", hexString);

			hexString = RtfUtility.UIntToUpperCaseHexString(0xff7f3f, 8);
			Assert.AreEqual("3F7FFF00", hexString);

			hexString = RtfUtility.UIntToUpperCaseHexString(0xff7f3f1f, 8);
			Assert.AreEqual("1F3F7FFF", hexString);
		}

		[Test]
		public void UIntToLowerCaseHexString()
		{
			string hexString = RtfUtility.UIntToLowerCaseHexString(0xff, 8);
			Assert.AreEqual("ff000000", hexString);

			hexString = RtfUtility.UIntToLowerCaseHexString(0xff7f, 8);
			Assert.AreEqual("7fff0000", hexString);

			hexString = RtfUtility.UIntToLowerCaseHexString(0xff7f3f, 8);
			Assert.AreEqual("3f7fff00", hexString);

			hexString = RtfUtility.UIntToLowerCaseHexString(0xff7f3f1f, 8);
			Assert.AreEqual("1f3f7fff", hexString);
		}
	}
}
