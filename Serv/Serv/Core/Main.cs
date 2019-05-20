using System;

namespace Network
{
	class MainClass
	{
        private void Debug()
        {
            // Debug ·þÎñÆ÷²âÊÔ
            DataMgr dataMgr = DataMgr.Instance;
            bool ret = dataMgr.Register("Anotts", "86696686");
            if (ret)
                Console.WriteLine("Register Success");
            else
                Console.WriteLine("Register Failure");
            // Register -> Create
            ret = dataMgr.CreatePlayer("Anotts");
            if (ret)
                Console.WriteLine("Create Success");
            else
                Console.WriteLine("Create Failure");
            // Get Data
            PlayerData pd = dataMgr.GetPlayerData("Anotts");
            if (pd != null)
                Console.WriteLine("Get Data Score is:" + pd.score);
            else
                Console.WriteLine("Get Data Failure");
            // Save Data
            pd.score += 10;
            //dataMgr.SavePlayer();
        }

        public static void Main(string[] args)
		{
            DataMgr dataMgr = DataMgr.Instance;
            ServNet servNet = ServNet.Instance;
			servNet.proto = new ProtocolBytes();
			servNet.Start("127.0.0.1", 8888);

			while(true)
			{
				string str = Console.ReadLine();
				switch(str)
				{
				case "quit":
					servNet.Close();
					return;
				case "print":
					servNet.Print();
					break;
				}
			}

		}
	}
}
