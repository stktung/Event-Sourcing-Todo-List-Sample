namespace SleekFlow.Todo.Domain;

public class DateTimeHelper
{
    public static DateTime? ConvertToUtc(DateTime? dt)
    {
        DateTime? dtInUtc = dt;
        if (dt != null)
        {
            switch (dt.Value.Kind)
            {
                case DateTimeKind.Local:
                    dtInUtc = dt.Value.ToUniversalTime();
                    break;
                case DateTimeKind.Unspecified:
                    dtInUtc = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
                    break;
            }
        }

        return dtInUtc;
    }

}