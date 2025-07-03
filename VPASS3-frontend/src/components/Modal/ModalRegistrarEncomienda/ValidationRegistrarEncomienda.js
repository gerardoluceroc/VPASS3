import * as Yup from 'yup';

/**
 * Esquema de validación para el formulario de registro de encomiendas.
 *
 * - `nombreDestinatario`: Requerido siempre.
 * - `codigoEncomienda`: Opcional, pero no puede contener caracteres especiales.
 * - `idZona` e `idDepartamento`: Requeridos siempre.
 * - `nombrePersonaQueRetira`, `apellidoPersonaQueRetira`, `rutPersonaQueRetira`: Requeridos solo si `encomiendaFueRetirada` es true.
 */

const ValidationRegistrarEncomienda = Yup.object().shape({
    nombreDestinatario: Yup.string()
        .trim()
        .required('El nombre del destinatario es requerido'),

    //   codigoEncomienda: Yup.string()
    //     .nullable()
    //     .matches(/^[a-zA-Z0-9\s-]*$/, 'El código no puede tener caracteres especiales'),
    codigoEncomienda: Yup.string()
        .nullable()
        .matches(/^[a-zA-Z0-9\s-_]*$/, 'El código solo puede contener letras, números, espacios, guiones y guiones bajos'),

    idZona: Yup.number()
        .required('Debe seleccionar una zona'),

    idDepartamento: Yup.number()
        .required('Debe seleccionar un departamento'),

    encomiendaFueRetirada: Yup.boolean(),

    nombrePersonaQueRetira: Yup.string()
        .nullable()
        .when('encomiendaFueRetirada', {
            is: true,
            then: (schema) => schema
            .trim()
            .required('Debe indicar el nombre de la persona que retira'),
        }),

    apellidoPersonaQueRetira: Yup.string()
        .nullable()
        .when('encomiendaFueRetirada', {
            is: true,
            then: (schema) => schema
            .trim()
            .required('Debe indicar el apellido de la persona que retira'),
        }),

    rutPersonaQueRetira: Yup.string()
        .nullable()
        .when('encomiendaFueRetirada', {
            is: true,
            then: (schema) => schema
            .trim()
            .required('Debe indicar el RUT de la persona que retira'),
        }),

    retiraPropietario: Yup.boolean(),
    });

export default ValidationRegistrarEncomienda;