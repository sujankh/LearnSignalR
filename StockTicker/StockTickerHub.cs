﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace StockTicker
{
    public class StockTicker
    {

    }
   
    public class StockTickerHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}