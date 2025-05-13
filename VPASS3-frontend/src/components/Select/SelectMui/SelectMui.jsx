import { Box, FormControl, FormHelperText, InputLabel, MenuItem, Select } from "@mui/material";

/* Componente Select de MaterialUI perzonalizado para ser re utilizado en otras partes. */
/* Props Entrada: 

- label: Este será el label que tendrá el Select a la hora de cargarse la primera vez.

- hanldeChange: Función que se ejecutará en caso de detectar un cambio o un valor seleccionado del select.

- listadoElementos: arreglo de objetos que será lo que se va a recorrer para mostrar los items del select.

- keyListadoElementos: atributo del arreglo de objetos que funcionará como key o id para recorrerlo y mostrar las opciones.

- mostrarElemento: Esta es una funcion que retorna lo que será mostrado del objeto en cada elemento seleccionable.
    Se escribe como funcion en caso de querer mostrar mas de un atributo del objeto. 
    Por ejemplo, si se quiere mostrar el nombre y apellido de cada persona, se podría escribir la prop como:

    mostrarElemento={(option) => `${option["nombres"]} ${option["apellidos"]}`}

    Donde 'option' representa un objeto del arreglo de objetos importado en la prop 'listadoElementos'


    En caso de solo querer mostrar solo un atributo, como los nombres por ejemplo, sería de esta manera.
    mostrarElemento={(option) => option["nombres"]}

- atributoValue: atributo del objeto que se usará como valor del Select una vez se selecciona una opción. Puede ser igual al atributo mostrable como no, eso depende del objetivo de la persona que utilice el componente.

- elementoSeleccionado: valor seleccionado en el Select, por defecto es null. Esto deberia ser un estado que se gestiona por parte del componente Padre que invoca al Select.

*/

const SelectMui = ({ 
    name = "", 
    label = "Seleccionar película", 
    handleChange = () => {}, 
    listadoElementos = [
        { idPelicula: 1, title: 'The Shawshank Redemption', year: 1994 },
        { idPelicula: 2, title: 'The Godfather', year: 1972 },
        { idPelicula: 3, title: 'The Godfather: Part II', year: 1974 },
        { idPelicula: 4, title: 'The Dark Knight', year: 2008 },
        { idPelicula: 5, title: '12 Angry Men', year: 1957 },
        { idPelicula: 6, title: "Schindler's List", year: 1993 },
        { idPelicula: 7, title: 'Pulp Fiction', year: 1994 }], 
    keyListadoElementos = "idPelicula", 
    atributoValue = "title", 
    elementoSeleccionado, 
    width = 150,
    mostrarElemento = (opcion) => `${opcion["title"]} ${opcion["year"]}`, 
    readOnly = false,
    disabled = false,
    disabledOptionCondition = (opcion) => {return false},
    helperText = "",
    error = false,
}) => {

    return (
        <Box sx={{ width: width }}>
            <FormControl fullWidth disabled={disabled}>
                <InputLabel id="demo-simple-select-label">{label}</InputLabel>
                <Select
                    name={name}
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    value={elementoSeleccionado === 0 ? "0" : elementoSeleccionado || ""}
                    label={label}
                    onChange={handleChange}
                    error={error}
                    fullWidth
                    inputProps={{ 
                        readOnly: readOnly,
                    }}
                    MenuProps={{
                        PaperProps: {
                          sx: {
                            maxHeight: '250px',
                            overflowY: 'auto',
                            width: '220px',
              
                            // Estilos para WebKit (Chrome, Safari, Edge) en el Paper (menú desplegable)
                            '&::-webkit-scrollbar': {
                              width: '8.024px !important',
                              background: 'transparent !important',
                            },
                            '&::-webkit-scrollbar-track': {
                              background: 'transparent !important',
                            },
                            '&::-webkit-scrollbar-thumb': {
                              background: '#C9C9C9',
                              borderRadius: '100px',
                            },
                          },
                        },
                    }}
                >
                    {listadoElementos.map((item) => (
                        <MenuItem disabled={disabledOptionCondition(item)} key={item[keyListadoElementos]} value={item[atributoValue]}>
                            {/* {JSON.stringify(item)} */}
                            {mostrarElemento(item)}
                        </MenuItem>
                    ))}
                </Select>
                {helperText && <FormHelperText error={error}>{helperText}</FormHelperText>}
            </FormControl>
        </Box>
    );
}

export default SelectMui;