using Microsoft.AspNetCore.Components;

namespace RafeTale.UI.Shared.Shared.Extensions.Interfaces
{
    public interface IDescriptionFormatter
    {
        MarkupString Format(string rawDescription, bool coolFormat = true);
    }
}
