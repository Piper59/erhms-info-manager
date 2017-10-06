namespace ERHMS.Test
{
    public static class TypeMaps
    {
        public static void Configure()
        {
            ConstantRepository.Configure();
            GenderRepository.Configure();
            PersonRepository.Configure();
        }
    }
}
