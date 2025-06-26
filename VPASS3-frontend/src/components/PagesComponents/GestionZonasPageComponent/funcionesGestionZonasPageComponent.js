export function eliminarDepartamentoFromRows(rows, idDepartamento) {
  return rows.map(row => {
    // Filtramos las apartments eliminando la que tenga el id igual a idDepartamento
    const updatedApartment = row.apartments?.filter(
      apartment => apartment.id !== idDepartamento
    ) || [];

    // Retornamos el row con las apartments actualizadas
    return {
      ...row,
      apartments: updatedApartment
    };
  });
}

export function eliminarZonaFromRowsById(rows, idZona) {
  return rows.filter(row => row.id !== idZona);
}

export function agregarDepartamento(rows, idZona, departamentoCreado) {
  return rows.map(zona => {
    if (zona.id === idZona) {
      // Retornamos una copia de la zona con la nueva subzona agregada
      return {
        ...zona,
        apartments: [...zona.apartments, departamentoCreado]
      };
    }
    return zona; // las demás zonas se retornan igual
  });
}