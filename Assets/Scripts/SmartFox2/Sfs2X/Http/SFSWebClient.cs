using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Sfs2X.Http
{
	public class SFSWebClient
	{
		private HttpResponseDelegate onHttpResponse;

		public HttpResponseDelegate OnHttpResponse
		{
			get
			{
				return onHttpResponse;
			}
			set
			{
				onHttpResponse = value;
			}
		}

		public void UploadValuesAsync(Uri uri, string paramName, string encodedData)
		{
			//Discarded unreachable code: IL_004d
			TcpClient tcpClient = null;
			try
			{
				IPAddress address = IPAddress.Parse(uri.Host);
				tcpClient = new TcpClient();
				tcpClient.Client.Connect(address, uri.Port);
			}
			catch (Exception ex)
			{
				OnHttpResponse(true, "Http error creating http connection: " + ex.ToString());
				return;
			}
			try
			{
				string text = paramName + "=" + encodedData;
				byte[] bytes = Encoding.UTF8.GetBytes(text);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("POST /BlueBox/BlueBox.do HTTP/1.0\r\n");
				stringBuilder.Append("Content-Type: application/x-www-form-urlencoded; charset=utf-8\r\n");
				stringBuilder.AppendFormat("Content-Length: {0}\r\n", bytes.Length);
				stringBuilder.Append("\r\n");
				stringBuilder.Append(text);
				StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream());
				streamWriter.Write(stringBuilder.ToString() + '\0');
				streamWriter.Flush();
				StringBuilder stringBuilder2 = new StringBuilder();
				byte[] array = new byte[4096];
				int num;
				while ((num = tcpClient.GetStream().Read(array, 0, 4096)) > 0)
				{
					byte[] array2 = new byte[num];
					Buffer.BlockCopy(array, 0, array2, 0, num);
					stringBuilder2.Append(Encoding.UTF8.GetString(array2));
					array = new byte[4096];
				}
				string[] array3 = Regex.Split(stringBuilder2.ToString(), "\r\n\r\n");
				if (array3.Length < 2)
				{
					OnHttpResponse(true, "Error during http response: connection closed by remote side");
					return;
				}
				string message = Regex.Replace(array3[1], "\\s+$", "");
				OnHttpResponse(false, message);
			}
			catch (Exception ex2)
			{
				OnHttpResponse(true, "Error during http request: " + ex2.ToString() + " " + ex2.StackTrace);
			}
			finally
			{
				try
				{
					tcpClient.Close();
				}
				catch (Exception ex3)
				{
					OnHttpResponse(true, "Error during http scocket shutdown: " + ex3.ToString() + " " + ex3.StackTrace);
				}
			}
		}
	}
}
