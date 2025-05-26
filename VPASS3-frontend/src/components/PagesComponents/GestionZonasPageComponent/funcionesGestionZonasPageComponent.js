export function eliminarSubZonaFromRows(rows, idSubZona) {
  return rows.map(row => {
    // Filtramos las zoneSections eliminando la que tenga el id igual a idSubZona
    const updatedZoneSections = row.zoneSections?.filter(
      zone => zone.id !== idSubZona
    ) || [];

    // Retornamos el row con las zoneSections actualizadas
    return {
      ...row,
      zoneSections: updatedZoneSections
    };
  });
}

export function eliminarZonaFromRows(rows, idZona) {
  return rows.filter(row => row.id !== idZona);
}