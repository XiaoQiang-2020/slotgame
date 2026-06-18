using YooAsset;

namespace Framework.Patch
{
    public static class PatchFacade
    {
        public static PatchModel Model { get; private set; }
        public static bool IsCreated => Model != null;

        public static void Create(string packageName, EPlayMode playMode)
        {
            if (IsCreated)
            {
                throw new System.InvalidOperationException("PatchFacade has already been created.");
            }

            Model = new PatchModel(packageName, playMode);
            PatchManager.Create(packageName, playMode);
        }

        public static void Start()
        {
            if (!IsCreated)
            {
                throw new System.InvalidOperationException("PatchFacade has not been created.");
            }

            PatchManager.Start();
        }

        public static void Update()
        {
            if (!IsCreated)
            {
                return;
            }

            PatchManager.Update();
        }
    }
}