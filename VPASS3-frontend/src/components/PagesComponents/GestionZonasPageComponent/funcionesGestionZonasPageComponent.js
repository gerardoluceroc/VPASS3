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

export function eliminarZonaFromRowsById(rows, idZona) {
  return rows.filter(row => row.id !== idZona);
}

export function agregarSubZona(rows, idZona, dataSubZonaAgregada) {
  return rows.map(zona => {
    if (zona.id === idZona) {
      // Retornamos una copia de la zona con la nueva subzona agregada
      return {
        ...zona,
        zoneSections: [...zona.zoneSections, dataSubZonaAgregada]
      };
    }
    return zona; // las demás zonas se retornan igual
  });
}