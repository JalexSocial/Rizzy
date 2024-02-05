using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Rizzy.Components.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Rizzy.Components.Form;

/// <summary>
/// A component that wraps the HTML file input element and supplies a Stream for each file's contents.
/// </summary>
public class RzInputFile : InputFile
{
}