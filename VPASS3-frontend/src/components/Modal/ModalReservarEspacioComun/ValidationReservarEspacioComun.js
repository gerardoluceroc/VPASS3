import * as Yup from 'yup';

const ValidationReservarEspacioComun = Yup.object().shape({
  idEspacioComunSeleccionado: Yup.string()
    .required('El campo es requerido'),

  nombres: Yup.string()
    .trim()
    .required('El campo es requerido'),
  apellidos: Yup.string()
    .trim()
    .required('El campo es requerido'),
  numeroIdentificacion: Yup.string()
    .trim()
    .required('El campo es requerido'),

  idTipoReservacion: Yup.string()
    .required("El campo es requerido"),

  idOpcionRadioFechaReserva: Yup.string()
    .required('El campo es requerido'),
  fechaReserva: Yup.string()
    .nullable()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
  horaReserva: Yup.string()
    .nullable()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
  minutosHoraReserva: Yup.string()
    .nullable()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
    
  cantidadHorasReserva: Yup.string()
    .required("El campo es requerido"),
  cantidadMinutosReserva: Yup.string()
    .required("El campo es requerido"),

  idOpcionRadioIncluyeInvitados: Yup.string()
    .required('El campo es requerido'),
  cantidadInvitados: Yup.string()
  .nullable()
  .when('idOpcionRadioIncluyeInvitados', {
    is: '1',
    then: (schema) =>
      schema
        .required('El campo es requerido')
        .matches(/^[1-9]\d*$/, 'Debe ser un número entero mayor a cero')
  }),
});

export default ValidationReservarEspacioComun;