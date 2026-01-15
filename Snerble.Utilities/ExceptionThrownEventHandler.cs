namespace Snerble.Utilities;

public delegate void ExceptionThrownEventHandler(object sender, Exception e);
public delegate void ExceptionThrownEventHandler<in T>(object sender, T e) where T : Exception;