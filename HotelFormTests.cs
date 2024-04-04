using DZHotelRoom.DBconnect;
using DZHotelRoom;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;
using System.Windows.Forms;
using Xunit;

namespace TestProjectHotel
{
    [TestClass]
    public class HotelFormTests
    {
        private Mock<HotelRoomContext> mockContext;
        private Mock<DbSet<Room>> mockRoomSet;
        private Mock<DbSet<Room>> mockDbSet;
        public HotelFormTests()
        {
            mockContext = new Mock<HotelRoomContext>();
            mockRoomSet = new Mock<DbSet<Room>>();
            mockDbSet = new Mock<DbSet<Room>>();
            mockContext.Setup(c => c.Rooms).Returns(mockRoomSet.Object);
        }

        [Fact]
        public void LoadAllRoomsIntoDataGridView_PopulatesDataGridView()
        {
            var rooms = new List<Room>
            {
                new Room { IdRoom = 1, Status = "свободно" },
                new Room { IdRoom = 2, Status = "занято" }
            };
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.AsQueryable().Provider);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.AsQueryable().Expression);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.AsQueryable().ElementType);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.AsQueryable().GetEnumerator());
            var form = new HotelForm(mockContext.Object);

            form.LoadAllRoomsIntoDataGridView();
        }

        [Fact]
        public void FilterAndDisplayRooms_FiltersRoomsByStatus()
        {
            var rooms = new List<Room>
            {
                new Room { IdRoom = 1, Status = "свободно" },
                new Room { IdRoom = 2, Status = "занято" },
                new Room { IdRoom = 3, Status = "свободно" }
            };
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(rooms.AsQueryable().Provider);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(rooms.AsQueryable().Expression);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(rooms.AsQueryable().ElementType);
            mockRoomSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(rooms.AsQueryable().GetEnumerator());
            var form = new HotelForm(mockContext.Object);

            form.FilterAndDisplayRooms("свободно");
        }
        [Fact]
        public void Constructor_WithDbContext_LoadsAllRooms()
        {
            mockDbSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(new List<Room>().AsQueryable().Provider);
            mockDbSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(new List<Room>().AsQueryable().Expression);
            mockDbSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(new List<Room>().AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(new List<Room>().AsQueryable().GetEnumerator());

            var mockContext = new Mock<HotelRoomContext>();
            mockContext.Setup(c => c.Rooms).Returns(mockDbSet.Object);

            var hotelForm = new HotelForm(mockContext.Object);
            mockDbSet.Verify(m => m.ToList(), Times.Once);
        }

        [Fact]
        public void SearchButton_Click_WithSearchText_FiltersRooms()
        {
            var hotelForm = new HotelForm(mockContext.Object);
            var searchText = "Иван";
            hotelForm.SearchButton_Click(null, EventArgs.Empty);
            mockDbSet.Verify(m => m.Where(It.IsAny<Expression<Func<Room, bool>>>()), Times.AtLeastOnce);
        }
        public void ReservedRadioButton_CheckedChanged_FiltersReservedRooms()
        {
            var hotelForm = new HotelForm(mockContext.Object);
            hotelForm.ReservedRadioButton_CheckedChanged(null, EventArgs.Empty);
            mockRoomSet.Verify(m => m.Where(r => r.Status == "зарезервировано"), Times.Once);
        }
        [Fact]
        public void ViewButton_Click_WithSelectedRoom_OpensViewForm()
        {
            var hotelForm = new HotelForm(mockContext.Object);
            hotelForm.idRoom = 1;
            hotelForm.ViewButton_Click(null, EventArgs.Empty);
        }

    }
}