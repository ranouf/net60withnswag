using ApiWithAuthentication.Domains.Core.Items;
using ApiWithAuthentication.Domains.Core.Items.Entities;
using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using Xunit.Abstractions;

namespace ApiWithAuthentication.Tests.Data
{
    public class TestItemDataBuilder : BaseDataBuilder
    {
        private readonly IItemManager _itemManager;

        public const string Item1 = "Item St Hubert";
        public const string Item2 = "Item St Laurent";
        public const string Item3 = "Item Sherbrooke";

        public TestItemDataBuilder(
            SKSamplesDbContext context,
            IItemManager ItemManager,
            ITestOutputHelper output
        ) : base(context, output)
        {
            _itemManager = ItemManager;
        }

        public override void Seed()
        {
            var items = new Item[]
            {
                new Item(Item1),
                new Item(Item2),
                new Item(Item3),
            };

            for (int i = 0; i < items.Length; i++)
            {
                var Item = items[i];
                _itemManager.CreateItemAsync(Item).Wait();
            }
            Output.WriteLine($"{items.Length} Items have been created.");
        }
    }
}
