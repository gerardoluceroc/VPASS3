import * as Yup from 'yup';
/**
 * Esquema de validación para el formulario de retirar encomienda.
 *
 * - `nombrePersonaQueRetira`, `apellidoPersonaQueRetira`, `rutPersonaQueRetira`: Requeridos siempre.
 * - `retiraPropietario`: Booleano opcional.
 */
const ValidationRetirarEncomienda = Yup.object().shape({

    nombrePersonaQueRetira: Yup.string()
        .nullable()
        .trim()
        .required('Debe indicar el nombre de la persona que retira'),

    apellidoPersonaQueRetira: Yup.string()
        .nullable()
        .trim()
        .required('Debe indicar el apellido de la persona que retira'),

    rutPersonaQueRetira: Yup.string()
        .nullable()
        .trim()
        .required('Debe indicar el RUT de la persona que retira'),

    retiraPropietario: Yup.boolean(),
    });

export default ValidationRetirarEncomienda;