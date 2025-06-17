// Ids de los sentidos que tienen una visita
export const idSentidoVisitaEntrada = 1;
export const idSentidoVisitaSalida = 2;

// Constantes para las horas mínimas y máximas de uso del estacionamiento que se usarán en el select
export const cantidadHorasMinimasUsoEstacionamiento = 0;
export const cantidadHorasMaximasUsoEstacionamiento = 24;

// Constantes para las horas mínimas y máximas de una reserva de un espacio común
export const cantidadHorasMinimasReserva = 0;
export const cantidadHorasMaximasReserva = 24;

// Arreglo con los modos que puede tener un espacio comun, hasta el momento son "usables", "reservables", o ambos a la vez.
// Esto tiene que ser igual al enums del backend llamado CommonAreaMode que utiliza flags en .Net.
// Asi que en caso de cambios en el backend, debe cambiarse en esta parte también para que el front y el back queden sincronizados.
export const CommonAreaMode = {
  None: 0,
  Usable: 1,        // 1 << 0
  Reservable: 2     // 1 << 1
};

//Ids de tipos de reserva de espacio comun
export const idReservacionTipoUso  = 1;
export const idReservacionTipoReserva = 2;
export const opcionesReservacionEspacioComun = [
  {
    id: idReservacionTipoUso,
    name: "Uso compartido"
  },
  {
    id: idReservacionTipoReserva,
    name: "Reserva exclusiva"
  }
]