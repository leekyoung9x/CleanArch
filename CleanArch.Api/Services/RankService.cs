using CleanArch.Core.Entities.RequestModel;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CleanArch.Api.Services
{
    public class RankService : IRankService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RankService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<bool> GetReward(int money, int playerId)
        {

            var baseURL = _configuration["Admin:BaseURL"];
            var key = _configuration["Admin:Key"];
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{baseURL}/api/Client/Reward?id={playerId}&keySecurrity={key}&amount={money}");
            var jsonString = await response.Content.ReadAsStringAsync();

            return true;
        }

        private static void GetRewardItem(int money, int playerId, List<send_item> items)
        {
            switch (money)
            {
                case 1000000:
                    {
                        items.Add(new send_item()
                        {
                            item_id = 1227,
                            player_id = playerId,
                            quantity = 20,
                            option_quantity = 1,
                            options = new List<item_shop_option>
                            {
                                new item_shop_option()
                                {
                                    option_id = 30,
                                    param = 0
                                }
                            }
                        });

                        items.Add(new send_item()
                        {
                            item_id = 1288,
                            player_id = playerId,
                            quantity = 200000,
                            option_quantity = 1,
                            options = new List<item_shop_option>
                            {
                                new item_shop_option()
                                {
                                    option_id = 30,
                                    param = 0
                                }
                            }
                        });

                        items.Add(new send_item()
                        {
                            item_id = 2109,
                            player_id = playerId,
                            quantity = 5,
                            option_quantity = 1,
                            options = new List<item_shop_option>
                            {
                                new item_shop_option()
                                {
                                    option_id = 30,
                                    param = 0
                                }
                            }
                        });
                        break;
                    }
                case 2000000:
                    {

                        break;
                    }
                case 5000000:
                    {
                        items.Add(new send_item()
                        {
                            item_id = 1288,
                            player_id = playerId,
                            quantity = 1000000,
                            option_quantity = 1,
                            options = new List<item_shop_option>
                            {
                                new item_shop_option()
                                {
                                    option_id = 30,
                                    param = 0
                                }
                            }
                        });

                        items.Add(new send_item()
                        {
                            item_id = 1230,
                            player_id = playerId,
                            quantity = 1,
                            option_quantity = 1,
                            options = new List<item_shop_option>
                            {
                                new item_shop_option()
                                {
                                    option_id = 77,
                                    param = 30
                                },
                                new item_shop_option()
                                {
                                    option_id = 103,
                                    param = 30
                                },
                                new item_shop_option()
                                {
                                    option_id = 50,
                                    param = 30
                                },
                            }
                        });
                        break;
                    }
                case 10000000:
                    {

                        break;
                    }
                default:
                    break;
            }
        }
    }
}
