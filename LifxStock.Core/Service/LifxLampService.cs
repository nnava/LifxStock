using LifxHttp;
using LifxStock.Core.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LifxStock.Core.Service
{
    public class LifxLampService
    {
        private static string TOKEN;

        public List<Light> Lamps { get; set; }

        public LifxLampService(string token)
        {
            TOKEN = token;
        }

        #region GetLamps

        public async Task SetAvailabeLampsToList()
        {
            Lamps = new List<Light>();

            await SetLampsToList();
        }

        private async Task SetLampsToList()
        {
            var lifxClient = new LifxClient(TOKEN);
            var lamps = await lifxClient.ListLights().ConfigureAwait(false);

            SetLampsToList(lamps);
        }

        private void SetLampsToList(IEnumerable<Light> lamps)
        {
            foreach (var lamp in lamps.Where(e => e.SecondsSinceSeen < 3))
            {
                AddLampToList(lamp);
            }
        }

        private void AddLampToList(Light lamp)
        {
            Lamps.Add(lamp);
        }

        #endregion

        public async Task SetStaticLampList()
        {
            var lifxClient = new LifxClient(TOKEN);

            try
            {
                var lampStatusList = new ObservableCollection<Lamp>();

                foreach (var light in await lifxClient.ListLights())
                {
                    var lampOnline = false;
                    if (light.IsConnected && light.SecondsSinceSeen < 3)
                    {
                        lampOnline = true;
                    }

                    lampStatusList.Add(new Lamp { LampId = light.UUID, Name = light.Label, Online = lampOnline });
                }

                StaticLampStatusList.lampStatusList = lampStatusList;
            }
            catch { };
        }

        #region SetColor

        public async Task SetLightAsync(LifxColor color, string lampId)
        {
            var lifxClient = new LifxClient(TOKEN);

            try
            {
                int i = 0;
                while (i < 5)
                {
                    foreach (var light in await lifxClient.ListLights())
                    {
                        if (light.IsConnected && light.SecondsSinceSeen < 3 && light.UUID == lampId)
                        {
                            await light.SetColor(color);
                            i = 5;
                        }
                    }

                    i++;
                }
            }
            catch { };
        }

        public static LifxColor GetColorByProcent(double changeProcentValue)
        {
            if (changeProcentValue == 0) return LifxColor.DefaultWhite;

            if (changeProcentValue > 0.01)
            {
                if (IsBetween(changeProcentValue, 0.01, 0.25))
                    return new LifxColor.HSB(125, 1.00f, 0.2f);
                else if (IsBetween(changeProcentValue, 0.26, 0.50))
                    return new LifxColor.HSB(125, 1.00f, 0.3f);
                else if (IsBetween(changeProcentValue, 0.51, 0.75))
                    return new LifxColor.HSB(125, 1.00f, 0.4f);
                else if (IsBetween(changeProcentValue, 0.76, 1.00))
                    return new LifxColor.HSB(125, 1.00f, 0.5f);
                else if (IsBetween(changeProcentValue, 1.01, 1.50))
                    return new LifxColor.HSB(125, 1.00f, 0.6f);
                else if (IsBetween(changeProcentValue, 1.51, 2.00))
                    return new LifxColor.HSB(125, 1.00f, 0.7f);
                else if (IsBetween(changeProcentValue, 2.01, 2.50))
                    return new LifxColor.HSB(125, 1.00f, 0.8f);
                else
                    return new LifxColor.HSB(125, 1.00f, 1.0f);
            }
            else
            {
                if (IsBetween(changeProcentValue, -0.25, -0.01))
                    return new LifxColor.HSB(359, 1.00f, 0.2f);
                else if (IsBetween(changeProcentValue, -0.50, -0.26))
                    return new LifxColor.HSB(359, 1.00f, 0.3f);
                else if (IsBetween(changeProcentValue, -0.75, -0.51))
                    return new LifxColor.HSB(359, 1.00f, 0.4f);
                else if (IsBetween(changeProcentValue, -1.00, -0.76))
                    return new LifxColor.HSB(359, 1.00f, 0.5f);
                else if (IsBetween(changeProcentValue, -1.50, -1.01))
                    return new LifxColor.HSB(359, 1.00f, 0.6f);
                else if (IsBetween(changeProcentValue, -2.00, -1.51))
                    return new LifxColor.HSB(359, 1.00f, 0.7f);
                else if (IsBetween(changeProcentValue, -2.50, -2.01))
                    return new LifxColor.HSB(359, 1.00f, 0.8f);
                else
                    return new LifxColor.HSB(359, 1.00f, 1.0f);
            }
        }

        private static bool IsBetween(double value, double Min, double Max)
        {
            if (value >= Min && value <= Max) return true;
            else return false;
        }

        #endregion
    }
}