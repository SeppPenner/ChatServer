// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UiThreadInvokeClass.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The UI thread invoke class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ChatClient;

/// <summary>
/// The UI thread invoke class.
/// </summary>
public static class UiThreadInvokeClass
{
    /// <summary>
    /// Calls the UI thread.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="code">The code.</param>
    public static void UiThread(this Control control, Action code)
    {
        if (control.InvokeRequired)
        {
            control.BeginInvoke(code);
            return;
        }

        code.Invoke();
    }

    /// <summary>
    /// Handles the UI thread invoke.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="code">The code.</param>
    public static void UiThreadInvoke(this Control control, Action code)
    {
        if (control.InvokeRequired)
        {
            control.Invoke(code);
            return;
        }

        code.Invoke();
    }
}
