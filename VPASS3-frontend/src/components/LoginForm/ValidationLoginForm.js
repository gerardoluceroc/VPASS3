import * as Yup from 'yup';

const ValidationLoginForm = Yup.object({
  email: Yup.string()
    .trim()  // Elimina los espacios al principio y al final
    .email('Por favor ingresa un correo electrónico válido')
    .required('El correo electrónico es obligatorio'),
  
  password: Yup.string()
    .trim()  // Elimina los espacios al principio y al final
    .min(6, 'La contraseña debe tener al menos 6 caracteres')
    .required('La contraseña es obligatoria'),
});


export default ValidationLoginForm;
