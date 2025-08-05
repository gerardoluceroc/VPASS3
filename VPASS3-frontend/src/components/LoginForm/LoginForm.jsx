import "./LoginForm.css";
import { useState } from "react";
import { Box, Checkbox, FormControlLabel, Typography } from "@mui/material";
import { useFormik } from "formik";
import useLogin from "../../hooks/auth/useLogin";
import ValidationLoginForm from "./ValidationLoginForm";
import TextFieldUno from "../TextField/TextFieldUno/TextFieldUno";
import ButtonTypeOne from "../Buttons/ButtonTypeOne/ButtonTypeOne";
import { useNavigate } from "react-router-dom";
import { RUTA_HOME } from "../../utils/rutasCliente";

const LoginForm = () => {

    const {response, loading, loginSession, logoutSession, responseStatus} = useLogin();
  
    const [cargando, setCargando] = useState(false);

    const navigate = useNavigate();
  
    const formik = useFormik({
      initialValues: {
        email: "",
        password: ""
      },
      validationSchema: ValidationLoginForm,
      onSubmit: async () => {
        try {
          const result = await loginSession({ email: formik.values.email, password: formik.values.password });
          if (result) {
            const { token, status } = result;
            if (token) {
              console.log("Login exitoso");

              navigate(RUTA_HOME, { replace: true });
            // logoutSession(); // Llama a la función logoutSession después de iniciar sesión
            

            } else if (status === 401) {
              formik.setFieldError('email', "Correo y/o contraseña incorrecto(s)");
              formik.setFieldError('password', "Correo y/o contraseña incorrecto(s)");
            }
          } else {
            console.error("Error al intentar iniciar sesión: Correo y/o contraseña incorrecto(s)");
            formik.setFieldError('email', "Error al iniciar sesión");
            formik.setFieldError('password', "Error al iniciar sesión");
          }
        } catch (error) {
          formik.setFieldError('email', "Hubo un error al intentar iniciar sesión");
          formik.setFieldError('password', "Hubo un error al intentar iniciar sesión");
        }
      }
    });
  
    return (
      <Box id="ContainerLogin">
        <Box id="BoxLogin">
          <Box id="HeaderLogin">
            <Typography variant="h4" color="black">Iniciar sesión</Typography>
            <Typography variant="h6" color="gray">Ingresa tus datos para iniciar sesión</Typography>
          </Box>
  
          <Box id="CredentialsLogin">
            <TextFieldUno
              label="Correo electrónico"
              placeholder="ejemplo@ejemplo.com"
              name="email"
              type="email"
              value={formik.values.email}
              onChange={formik.handleChange}
              error={formik.touched.email && Boolean(formik.errors.email)}
              helperText={formik.touched.email && formik.errors.email}
            />
  
            <TextFieldUno
              label="Contraseña"
              placeholder=""
              name="password"
              type="password"
              value={formik.values.password}
              onChange={formik.handleChange}
              error={formik.touched.password && Boolean(formik.errors.password)}
              helperText={formik.touched.password && formik.errors.password}
            />
  
            <FormControlLabel control={<Checkbox defaultChecked />} label="Recuerdame" sx={{marginLeft: "5px", marginTop: "-15px", color: "black"}} />
  
          </Box>
  
          <Box id="BoxLoginButton">
            <ButtonTypeOne
            defaultText="Iniciar sesión"
            loadingText="Iniciando sesión..."
            handleClick={formik.handleSubmit}
            loading={loading}
            />
          </Box>
        </Box>
  
      </Box>
    )
  }
  
  export default LoginForm;