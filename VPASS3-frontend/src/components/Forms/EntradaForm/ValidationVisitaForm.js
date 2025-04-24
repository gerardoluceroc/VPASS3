import * as Yup from 'yup';

export const ValidationVisitaForm = Yup.object().shape({
    nombres: Yup.string().required('El campo es requerido'),
    apellidos: Yup.string().required('El campo es requerido'),
    rut: Yup.string().required('El campo es requerido'),

    idTipoVisita: Yup.number()
    .typeError('El campo es requerido')
    .required('El campo es requerido'),

    idZona: Yup.number()
    .typeError('El campo es requerido')
    .required('El campo es requerido'),

    idSubZona: Yup.number()
    .typeError('El campo es requerido')
    .required('El campo es requerido'),

    idSentido: Yup.number()
    .typeError('El campo es requerido')
    .required('El campo es requerido'),

    incluyeVehiculo: Yup.boolean(),

    patenteVehiculo: Yup.string()
    .nullable()
    .when('incluyeVehiculo', {
    is: true,
    then: (schema) =>
        schema.required('El campo es requerido').min(1, 'El campo es requerido'),
    }),

    idEstacionamiento: Yup.number()
    .nullable()
    .when('incluyeVehiculo', {
        is: true,
        then: (schema) =>
        schema.typeError('El campo es requerido').required('El campo es requerido'),
    }),

});
