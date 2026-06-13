namespace UBahnApi.DTOs;

public record StationCreateDto(string Name, int LinieId, int Position);
public record StationUpdateDto(string Name, int LinieId, int Position);
public record StationDto(int Id, string Name, int LinieId, string LinieName, int Position);
