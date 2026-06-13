namespace UBahnApi.DTOs;

public record LinieCreateDto(string Name);
public record LinieUpdateDto(string Name);

public record StationInLinieDto(int Id, string Name, int Position);

public record LinieDetailDto(int Id, string Name, List<StationInLinieDto> Stationen);
