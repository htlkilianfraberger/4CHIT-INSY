using System;
using System.Collections.Generic;

namespace Model;

public partial class Demos : IHasId
{
    public int Id { get; set; }

    public string? Value { get; set; }
}
