﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Sider.Serialization;

namespace Sider.Tests
{
  public class RedisReaderTests : SiderTestBase
  {
    public const int ReadBulkTestTimeout = 5000;


    private RedisReader createReader(string data)
    {
      return createReader(Encoding.Default.GetBytes(data));
    }

    private RedisReader createReader(byte[] data)
    {
      return createReader(new MemoryStream(data));
    }

    private RedisReader createReader(Stream s)
    {
      return new RedisReader(s, RedisSettings.Default);
    }


    [Test]
    public void Ctor_StreamIsNull_ExceptionThrown()
    {
      Assert.Throws<ArgumentNullException>(() => new RedisReader(null));
    }

    [Test]
    public void Ctor_StreamUnreadble_ExceptionThrown()
    {
      var tempFile = Path.GetTempFileName();
      var fs = File.OpenWrite(tempFile);

      try { Assert.Throws<ArgumentException>(() => new RedisReader(fs)); }
      finally {
        fs.Close();
        fs.Dispose();

        File.Delete(tempFile);
      }
    }

    [Test]
    public void Ctor_SettingsIsNull_ExceptionThrown()
    {
      Assert.Throws<ArgumentNullException>(() =>
        new RedisReader(new MemoryStream(new byte[] { 0xFF }), null));
    }


    #region ReadTypeChar
    [Test]
    public void ReadTypeChar_AllTypeChars_CorrectTypeReturned()
    {
      var typeChars = Enum
        .GetValues(typeof(ResponseType))
        .OfType<ResponseType>()
        .Select(c => (char)c)
        .ToArray();

      var data = typeChars.Aggregate("", (c1, c2) => c1 + c2);
      var reader = createReader(data);

      foreach (var t in typeChars)
        Assert.That(reader.ReadTypeChar(), Is.EqualTo((ResponseType)t));
    }

    [Test]
    public void ReadTypeChar_InvalidData_ExceptionThrown()
    {
      Assert.Throws<ResponseException>(() =>
      {
        var reader = createReader("RANDOM_JUNK");
        reader.ReadTypeChar();
      });
    }

    [Test]
    public void ReadTypeChar_EmptyData_ExceptionThrown()
    {
      Assert.Throws<ResponseException>(() =>
      {
        var reader = createReader("");
        reader.ReadTypeChar();
      });
    }
    #endregion

    #region ReadNumberLine
    [Test]
    public void ReadNumberLine_EmptyData_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("");
    }

    [Test]
    public void ReadNumberLine_JustCrLf_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("\r\n");
    }

    [Test]
    public void ReadNumberLine_ZeroWithNoCrLf_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("0");
    }

    [Test]
    public void ReadNumberLine_ZeroWithCrButNoLf_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("0\r");
    }

    [Test]
    public void ReadNumberLine_ZeroWithLfButNoCr_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("0\n");
    }

    [Test]
    public void ReadNumberLine_LineWithZero_ZeroReturned()
    {
      var reader = createReader("0\r\n");
      var num = reader.ReadNumberLine();

      Assert.That(num, Is.EqualTo(0));
    }

    [Test]
    public void ReadNumberLine_GarbledData_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("SDOIJFOEIWJF\r\n");
    }

    [Test]
    public void ReadNumberLine_SomeGarbledDigit_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("123G456\r\n");
    }

    [Test]
    public void ReadNumberLine_NegativeNumWithSomeGarbledDigit_ExceptionThrown()
    {
      readNumberLine_exceptionTest<ResponseException>("-123G456\r\n");
    }

    [Test]
    public void ReadNumberLine_LineWithNegativeZero_ZeroReturned()
    {
      var reader = createReader("-0\r\n");
      var num = reader.ReadNumberLine();

      Assert.That(num, Is.EqualTo(0));
    }

    [Test]
    public void ReadNumberLine_LineWithSingleDigit_DigitReturned()
    {
      readNumberLine_parsingTest(Enumerable.Range(0, 10));
    }

    [Test]
    public void ReadNumberLine_PositiveNumbers_CorrectNumberReturned()
    {
      readNumberLine_parsingTest(Enumerable
        .Range(1, 1000)
        .Where(n => n % 7 == 0));
    }

    [Test]
    public void ReadNumberLine_NegativeNubmers_CorrectNumberReturned()
    {
      readNumberLine_parsingTest(Enumerable
        .Range(1, 1000)
        .Where(n => n % 6 == 0)
        .Select(n => -n));
    }


    private void readNumberLine_exceptionTest<TException>(string data)
      where TException : Exception
    {
      Assert.Throws<TException>(() =>
      {
        var reader = createReader(data);
        reader.ReadNumberLine(); // expected exception
      });
    }

    private void readNumberLine_parsingTest(IEnumerable<int> nums)
    {
      // put each number on its own line with CRLF
      var numString = string.Join("\r\n", nums) + "\r\n";
      var reader = createReader(numString);

      // parse each line as numbers, they should correspond to
      // the input sequence (in exactly the same ordering)
      foreach (var num in nums)
        Assert.That(reader.ReadNumberLine(), Is.EqualTo(num));
    }
    #endregion

    #region ReadStatusLine
    [Test]
    public void ReadStatusLine_EmptyData_ExceptionThrown()
    {
      readStatusLine_exceptionTest<ResponseException>("");
    }

    [Test]
    public void ReadStatusLine_JustCrLf_EmptyStringReturned()
    {
      var reader = createReader("\r\n");
      var str = reader.ReadStatusLine();

      Assert.That(str, Is.Empty);
    }

    [Test]
    public void ReadStatusLine_OneCharWithNoCrLf_ExceptionThrown()
    {
      readStatusLine_exceptionTest<ResponseException>("X");
    }

    [Test]
    public void ReadStatusLine_OneCharWithJustCr_ExceptionThrown()
    {
      readStatusLine_exceptionTest<ResponseException>("X\r");
    }

    [Test]
    public void ReadStatusLine_OneCharWithJustLf_ExceptionThrown()
    {
      readStatusLine_exceptionTest<ResponseException>("X\n");
    }

    [Test]
    public void ReadStatusLine_OneCharWithCrLf_OneCharReturned()
    {
      readStatusLine_parsingTest("X");
    }

    [Test]
    public void ReadStatusLine_IncreasinglyLargerString_CorrectStringReturned()
    {
      var chars = "abcdefghijklmnopqrstuvwxyz" +
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "0123456789";

      readStatusLine_parsingTest(Enumerable
        .Range(0, 100)
        .Select(n => new string(chars[n % chars.Length], n))
        .ToArray());
    }


    private void readStatusLine_exceptionTest<TException>(string data)
      where TException : Exception
    {
      Assert.Throws<TException>(() =>
      {
        var reader = createReader(data);
        reader.ReadStatusLine();
      });
    }

    private void readStatusLine_parsingTest(params string[] testStr)
    {
      var data = string.Join("\r\n", testStr) + "\r\n";
      var reader = createReader(data);

      foreach (var line in testStr) {
        Assert.That(reader.ReadStatusLine(), Is.EqualTo(line));
      }
    }
    #endregion

    #region ReadBulk
    [Test]
    public void ReadBulk_LengthIsNegative_ExceptionThrown()
    {
      readBulk_exceptionTest<ArgumentOutOfRangeException>("", -1);
    }

    [Test]
    public void ReadBulk_ZeroLengthData_EmptyBufferReturned()
    {
      var reader = createReader("\r\n");
      var buffer = reader.ReadBulk(0);

      Assert.That(buffer.Length, Is.EqualTo(0));
    }

    [Test]
    public void ReadBulk_LengthIsPositive_BufferWithSameLengthReturned()
    {
      var reader = createReader("0123456789\r\n");
      var buffer = reader.ReadBulk(10);

      Assert.That(buffer.Length, Is.EqualTo(10));
    }

    [Test]
    public void ReadBulk_LengthIsPositiveAndDataIsEnough_CorrectDataReturned()
    {
      var testStr = "0123456789abcdefghijklmnopqrstuvwxyz";
      var reader = createReader(testStr + "\r\n");
      var buffer = reader.ReadBulk(testStr.Length);

      Assert.That(buffer.Length, Is.EqualTo(testStr.Length));
      Assert.That(Encoding.Default.GetString(buffer), Is.EqualTo(testStr));
    }

    [Test]
    public void ReadBulk_NonStringData_CorrectDataReturned()
    {
      var data = new byte[] { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
      var crlf = new byte[] { 0x0D, 0x0A };

      var reader = createReader(data.Concat(crlf).ToArray());
      var buffer = reader.ReadBulk(data.Length);

      Assert.That(buffer.Length, Is.EqualTo(data.Length));
      Assert.That(buffer, Is.EquivalentTo(data));
    }

    [Test, Timeout(ReadBulkTestTimeout)]
    public void ReadBulk_LengthIsPositiveButNotEnoughData_ExceptionThrown()
    {
      readBulk_exceptionTest<ResponseException>("012345/r/n", 99);
    }

    [Test]
    public void ReadBulk_ReallyLargeData_CorrectDataReturned()
    {
      var data = new byte[4096];
      var crlf = new byte[] { 0x0D, 0x0A };
      (new Random()).NextBytes(data);

      var reader = createReader(data.Concat(crlf).ToArray());
      var buffer = reader.ReadBulk(data.Length);

      Assert.That(buffer.Length, Is.EqualTo(data.Length));
      Assert.That(buffer, Is.EquivalentTo(data));
    }
    
    [Test]
    public void ReadBulk_DataWithLotsOfCrLf_CorrectDataReturned()
    {
      var data = new byte[] {
        0x0D, 0x0A, 0xFF, 0x0D, 0x0A, 0xFF,
        0x0D, 0x0A, 0x0D, 0x0A, 0x0D, 0x0A,
      };
      var crlf = new byte[] { 0x0D, 0x0A };

      var buffer = new byte[data.Length];
      var reader = createReader(data.Concat(crlf).ToArray());
      reader.ReadBulk(buffer, 0, data.Length);

      Assert.That(buffer, Is.EquivalentTo(data));
    }

    [Test]
    public void ReadBulk_DataWithoutCrlf_ExceptionThrown()
    {
      readBulk_exceptionTest<ResponseException>("asdf", 4);
    }


    [Test]
    public void ReadBulk_WithBuffer_NegativeOffset_ExceptionThrown()
    {
      readBulk_withBuffer_exceptionTest<ArgumentOutOfRangeException>("asdf", -1, 4);
    }

    [Test]
    public void ReadBulk_WithBuffer_TooLargeOffset_ExceptionThrown()
    {
      readBulk_withBuffer_exceptionTest<ArgumentOutOfRangeException>("asdf", 99, 4);
    }

    [Test]
    public void ReadBulk_WithBuffer_TooLargeBulkLength_ExceptionThrown()
    {
      readBulk_withBuffer_exceptionTest<ArgumentException>("0123456\r\n", 0, 999);
    }

    [Test]
    public void ReadBulk_WithBuffer_LengthIsNegative_ExceptionThrown()
    {
      readBulk_withBuffer_exceptionTest<ArgumentOutOfRangeException>("asdf", 0, -1);
    }

    [Test]
    public void ReadBulk_WithBuffer_BufferIsNull_ExceptionThrown()
    {
      Assert.Throws<ArgumentNullException>(() =>
      {
        byte[] buffer = null;
        var reader = createReader("asdf");
        reader.ReadBulk(buffer, 0, 4);
      });
    }

    [Test]
    public void ReadBulk_WithBuffer_OffsetAndLengthIsZero_EmptyBufferReturned()
    {
      var buffer = new byte[0];
      var reader = createReader("\r\n");

      reader.ReadBulk(buffer, 0, 0);
    }

    [Test]
    public void ReadBulk_WithBuffer_LengthIsPositiveAndDataIsEnough_CorrectDataReturned()
    {
      var testStr = "012345";
      var buffer = new byte[6];
      var reader = createReader(testStr + "\r\n");

      reader.ReadBulk(buffer, 0, 6);

      Assert.That(Encoding.Default.GetString(buffer), Is.EqualTo(testStr));
    }

    [Test]
    public void ReadBulk_WithBuffer_ReallyLargeData_CorrectDataReturned()
    {
      var data = new byte[4096];
      var crlf = new byte[] { 0x0D, 0x0A };
      (new Random()).NextBytes(data);

      var reader = createReader(data.Concat(crlf).ToArray());
      var buffer = reader.ReadBulk(data.Length);

      Assert.That(buffer, Is.EquivalentTo(data));
    }


    private void readBulk_exceptionTest<TException>(string data, int length)
      where TException : Exception
    {
      Assert.Throws<TException>(() =>
      {
        var reader = createReader(data);
        reader.ReadBulk(length);
      });
    }

    private void readBulk_withBuffer_exceptionTest<TException>(string data, int offset, int length)
      where TException : Exception
    {
      Assert.Throws<TException>(() =>
      {
        var buffer = new byte[data.Length];
        var reader = createReader(data);
        reader.ReadBulk(buffer, offset, length);
      });
    }
    #endregion

    #region ReadBulkTo
    [Test]
    public void ReadBulkTo_StreamIsNull_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ArgumentNullException>(
        "0123\r\n", null, 4);
    }

    [Test]
    public void ReadBulkTo_LengthIsNegative_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ArgumentOutOfRangeException>(
        "0123\r\n", new MemoryStream(), -1);
    }

    [Test]
    public void ReadBulkTo_NegativeBufferSize_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ArgumentOutOfRangeException>(
        "012345\r\n", new MemoryStream(), 6, -1);
    }

    [Test]
    public void ReadBulkTo_DataWithNoCrLf_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ResponseException>(
        "012345", new MemoryStream(), 6);
    }

    [Test]
    public void ReadBulkTo_DataWithJustCr_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ResponseException>(
        "012345\r", new MemoryStream(), 6);
    }

    [Test]
    public void ReadBulkTo_DataWithJustLf_ExceptionThrown()
    {
      readBulkTo_withStream_exceptionTest<ResponseException>("012345\n",
        new MemoryStream(), 6);
    }

    [Test]
    public void ReadBulkTo_OutputStreamFailedMidway_ExceptionPreservedAndThrown()
    {
      var testStream = new TestExceptionStream(5, new MyException());
      readBulkTo_withStream_exceptionTest<MyException>("0123456789\r\n", testStream, 10);
    }

    [Test]
    public void ReadBulkTo_LengthIsZero_StreamContainsJustCrLf()
    {
      var reader = createReader("\r\n");
      var ms = new MemoryStream();

      reader.ReadBulkTo(ms, 0);

      Assert.That(ms.Length, Is.EqualTo(0));
      ms.Dispose();
    }

    [Test]
    public void ReadBulkTo_LengthIsPositive_StreamContainsDataWithSameLength()
    {
      var reader = createReader("012345\r\n");
      var ms = new MemoryStream();

      reader.ReadBulkTo(ms, 6);

      Assert.That(ms.Length, Is.EqualTo(6));
      ms.Dispose();
    }

    [Test]
    public void ReadBulkTo_ValidArgs_StreamContainsCorrectData()
    {
      var testStr = "012345";
      var reader = createReader(testStr + "\r\n");

      var ms = new MemoryStream();
      reader.ReadBulkTo(ms, testStr.Length);

      Assert.That(ms.Length, Is.EqualTo(testStr.Length));

      ms.Seek(0, SeekOrigin.Begin);
      var sr = new StreamReader(ms);
      var resultStr = sr.ReadToEnd();

      Assert.That(resultStr, Is.EqualTo(testStr));
    }


    private void readBulkTo_withStream_exceptionTest<TException>(string data, Stream s, int length,
      int? bufferSize = null)
      where TException : Exception
    {
      Assert.Throws<TException>(() =>
      {
        var reader = createReader(data);
        if (bufferSize.HasValue)
          reader.ReadBulkTo(s, length, bufferSize.Value);
        else
          reader.ReadBulkTo(s, length);
      });
    }
    #endregion

    #region ReadSerializedBulk

    [Test]
    public void ReadSerailizedBulk_SerializerIsNull_ExceptionThrown()
    {
      Assert.Throws<ArgumentNullException>(() =>
        createReader("hello").ReadSerializedBulk<string>(null, 99));
    }

    [Test]
    public void ReadSerializedBulk_BulkLengthIsNegative_ExceptionThrown()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => createReader("hello")
        .ReadSerializedBulk<string>(new Mock<ISerializer<string>>().Object, -1));
    }

    [Test]
    public void ReadSerializedBulk_ValidSerializer_SerializerReadCalled()
    {
      var testStr = "\r\n"; // should be about 7-10 bytes
      var mock = new Mock<ISerializer<string>>();
      mock.SetupAllProperties();
      mock.Setup(s => s.Read(It.IsAny<Stream>(), 5));

      createReader(testStr).ReadSerializedBulk<string>(mock.Object, 5);
      mock.VerifyAll();
    }

    [Test]
    public void ReadSerializedBulk_ValidSerializer_SerializerResultReturned()
    {
      var testStr = "hello\r\n";
      var expectedValue = "Not hello";

      var mock = new Mock<ISerializer<string>>();
      mock.SetupAllProperties();
      mock.Setup(s => s.Read(It.Is<Stream>(stream =>
        stream.ReadByte() == 'h' &&
        stream.ReadByte() == 'e' &&
        stream.ReadByte() == 'l' &&
        stream.ReadByte() == 'l' &&
        stream.ReadByte() == 'o'), 3))
        .Returns(expectedValue);

      var result = createReader(testStr).ReadSerializedBulk(mock.Object, 3);
      Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ReadSerializedBulk_StringData_DataPassedToSerializer()
    {
      var buffer = new byte[] { 0x0B, 0x0E, 0x0E, 0x0F, 0x0D, 0x0A };
      var mock = new Mock<ISerializer<string>>();
      mock.Setup(m => m.Read(It.Is<Stream>(s =>
        s.ReadByte() == 0x0B &&
        s.ReadByte() == 0x0E &&
        s.ReadByte() == 0x0E &&
        s.ReadByte() == 0x0F), 4));

      createReader(buffer).ReadSerializedBulk(mock.Object, 4);
      mock.VerifyAll();
    }

    #endregion
  }
}
