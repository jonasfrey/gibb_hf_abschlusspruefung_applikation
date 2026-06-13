namespace UBahnApi.DTOs;

public record FahrzeitCreateDto(int VonStationId, int NachStationId, int DauerMinuten);
public record FahrzeitUpdateDto(int VonStationId, int NachStationId, int DauerMinuten);
public record FahrzeitDto(int Id, int VonStationId, string VonStationName, int NachStationId, string NachStationName, int DauerMinuten);
