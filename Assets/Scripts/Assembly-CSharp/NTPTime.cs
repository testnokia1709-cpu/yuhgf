using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NTPTime
{
	public static DateTime GetTime()
	{
		try
		{
			byte[] array = new byte[48];
			array[0] = 27;
			IPAddress[] addressList = Dns.GetHostEntry("pool.ntp.org").AddressList;
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Connect(new IPEndPoint(addressList[0], 123));
			socket.ReceiveTimeout = 1000;
			socket.Send(array);
			socket.Receive(array);
			socket.Close();
			ulong num = ((ulong)array[40] << 24) | ((ulong)array[41] << 16) | ((ulong)array[42] << 8) | array[43];
			ulong num2 = ((ulong)array[44] << 24) | ((ulong)array[45] << 16) | ((ulong)array[46] << 8) | array[47];
			double ntpTime = num * 1000 + num2 * 1000 / 4294967296L;
			return ConvertToDateTime(ntpTime);
		}
		catch (Exception message)
		{
			Debug.Log("Could not get NTP time");
			Debug.Log(message);
			return DateTime.Now;
		}
	}

	private static DateTime ConvertToDateTime(double ntpTime)
	{
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(ntpTime);
		DateTime dateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return dateTime + timeSpan;
	}
}
