namespace CloudOnce.Internal
{
	public enum KvStoreChangeReason
	{
		ServerChange = 0,
		InitialSyncChange = 1,
		QuotaViolationChange = 2,
		AccountChange = 3
	}
}
