using ApiWithAuthentication.Servers.API.Controllers.Dtos.Paging;
using ApiWithAuthentication.Servers.API.Controllers.Items.Dtos;
using ApiWithAuthentication.Tests.Extensions;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Common = ApiWithAuthentication.Librairies.Common;

namespace ApiWithAuthentication.Tests.Controllers.Items
{
    [Collection(Constants.TEST_COLLECTION)]
    public class Items_Tests : BaseTest
    {
        public const string NewItem = "NewItem";
        public const string EditedItem = "EditedItem";

        public Items_Tests(
            ITestOutputHelper output
        ) : base(output) { }

        [Fact]
        public async Task Should_List_Items()
        {
            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            var response = await Client.GetAsync(
                Common.Constants.Api.V1.Items.Url,
                Output,
                new PagedRequestDto
                {
                    MaxResultCount = 1,
                    SkipCount = 0
                }
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.ConvertToAsync<PagedResultDto<ItemDto>>(Output);
            Assert.NotNull(dto);
            Assert.Equal(3, dto.TotalCount);
            Assert.Equal(1, dto.Items.Count);
            Assert.True(dto.HasNext);
        }

        [Fact]
        public async Task Should_Create_Then_Update_And_Delete_Item()
        {
            // As Admin
            await Client.AuthenticateAsAdministratorAsync(Output);

            // Create Task
            var insertItemRequest = new UpsertItemRequest()
            {
                Name = NewItem
            };

            var response = await Client.PostAsync(
                 Common.Constants.Api.V1.Items.Url,
                 Output,
                 insertItemRequest
             );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var newItem = await response.ConvertToAsync<ItemDto>(Output);
            Assert.Equal(NewItem, newItem.Name);

            // Edit Item
            var updateItemRequest = new UpsertItemRequest()
            {
                Name = EditedItem
            };

            response = await Client.PutByIdAsync(
                Common.Constants.Api.V1.Items.Url,
                Output,
                newItem.Id,
                updateItemRequest
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var editedItem = await response.ConvertToAsync<ItemDto>(Output);
            Assert.Equal(EditedItem, editedItem.Name);


            // Delete Item
            response = await Client.DeleteByIdAsync(
                Common.Constants.Api.V1.Items.Url,
                Output,
                newItem.Id
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
