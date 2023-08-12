namespace Application.Web.Api.JellyFishBackend
{
    public sealed class Const
    {
        private static Const _const;
        public readonly Guid DefaultUserType = Guid.Parse("7340425e-a5b5-11eb-bac0-309c2364fdb6");

        public static Const GetConst()
        {
            if(_const == null)  
                _const = new Const();
            return _const;
        }
    }
    
}
