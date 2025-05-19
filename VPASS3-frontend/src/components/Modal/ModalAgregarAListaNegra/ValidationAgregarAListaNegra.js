import * as Yup from 'yup';

export const ValidationAgregarAListaNegra = Yup.object().shape({
    nombres: Yup.string().required('El campo es requerido'),
    apellidos: Yup.string().required('El campo es requerido'),
    numeroIdentificacion: Yup.string().required('El campo es requerido'),
});