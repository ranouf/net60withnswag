namespace SK.Events
{
    public class Event : IEvent
    {
        public string Action
        {
            get
            {
                var type = this.GetType();
                return type.Name.EndsWith("Event")
                    ? type.Name.Replace("Event", string.Empty)
                    : type.Name;
            }
        }
    }
}
