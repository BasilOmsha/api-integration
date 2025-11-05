namespace api_integration.Application.src.FactoryInterfaces.Fingrid
{
    public interface IApiMapperFactory<in TApiResponse, out TReadDto>
    {
        TReadDto MapFromApiResponse(TApiResponse apiResponse);
    }
}