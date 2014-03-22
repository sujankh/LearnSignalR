using System;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;

namespace MoveShape
{
    public class ShapeBroadCaster
    {
        private static Lazy<ShapeBroadCaster> singleton = 
            new Lazy<ShapeBroadCaster>(() => new ShapeBroadCaster());

        private readonly TimeSpan BroadCastInterval = TimeSpan.FromMilliseconds(40);

        private bool shouldUpdateShape;

        private IHubContext shapeHub;
        private ShapeModel shapeModel;

        public ShapeBroadCaster()
        {
            var broadCastLoop = new Timer(UpdateShape, null, BroadCastInterval, BroadCastInterval);
            shouldUpdateShape = false;

            shapeHub = GlobalHost.ConnectionManager.GetHubContext<MoveShapeHub>();
        }

        private void UpdateShape(object state)
        {
            if (shouldUpdateShape)
            {
                shapeHub.Clients.AllExcept(shapeModel.LastUpdatedBy).updateShape(shapeModel);

                shouldUpdateShape = false;
            }
        }

        public void UpdateModel(ShapeModel shapeModel)
        {
            this.shapeModel = shapeModel;
            shouldUpdateShape = true;
            UpdateShape(null);
        }

        public static ShapeBroadCaster Instance
        {
            get { return singleton.Value; }
        }

    }

    public class MoveShapeHub : Hub
    {
        private ShapeBroadCaster shapeBroadCaster;
        public MoveShapeHub()
        {
            this.shapeBroadCaster = ShapeBroadCaster.Instance;
        }
        public void UpdateModel(ShapeModel clientModel)
        {
            clientModel.LastUpdatedBy = Context.ConnectionId;            
            shapeBroadCaster.UpdateModel(clientModel);
        }
    }

    public class ShapeModel
    {
        [JsonProperty("left")]
        public double Left { get; set; }

        [JsonProperty("top")]
        public double Top { get; set; }

        [JsonIgnore]
        public string LastUpdatedBy { get; set; }

    }
}