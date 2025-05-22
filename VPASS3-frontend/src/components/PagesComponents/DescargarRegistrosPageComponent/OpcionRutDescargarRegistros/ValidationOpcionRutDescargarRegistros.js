import * as Yup from 'yup';

export const ValidationOpcionRutDescargarRegistros = Yup.object().shape({
    numeroIdentificacion: Yup.string().required('El campo es requerido'),
});