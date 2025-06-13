import * as Yup from 'yup';

const ValidationReservarEspacioComun = Yup.object().shape({
  nombres: Yup.string()
    .trim()
    .required('El campo es requerido'),
  apellidos: Yup.string()
    .trim()
    .required('El campo es requerido'),
  numeroIdentificacion: Yup.string()
    .trim()
    .required('El campo es requerido'),

  idEspacioComunSeleccionado: Yup.string()
    .required('El campo es requerido'),

  idOpcionRadioFechaReserva: Yup.string()
    .required('El campo es requerido'),
  fechaReserva: Yup.string()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
  horaReserva: Yup.string()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
  minutosHoraReserva: Yup.string()
    .when('idOpcionRadioFechaReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),

  idOpcionRadioHorasReserva: Yup.string()
    .required('El campo es requerido'),
  cantidadHorasReserva: Yup.string()
    .when('idOpcionRadioHorasReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),
  cantidadMinutosReserva: Yup.string()
    .when('idOpcionRadioHorasReserva', {
      is: '2',
      then: (schema) => schema.required('El campo es requerido')
    }),

  idOpcionRadioIncluyeInvitados: Yup.string()
    .required('El campo es requerido'),
  cantidadInvitados: Yup.string()
    .when('idOpcionRadioIncluyeInvitados', {
      is: '1',
      then: (schema) => schema.required('El campo es requerido')
    }),
});

export default ValidationReservarEspacioComun;
