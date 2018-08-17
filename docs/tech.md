- Use imperative shell, function core style, with core being funcitonal interface with a GameState returned
- Use [VinylCutter](https://github.com/chamons/VinylCutter) to generate record types
- Drive design with unit tests until fleshed out enough for simple console app
- Decide later on Ui tech.


## Vim Tricks

I've stolen from myself the sepcial maekfile targets and vim script from VinylCutter.

- `nnoremap <leader>r :w<cr>:let $TEST_FILES=expand('%')<cr>:!make test-fast<cr>` - This builds tests and runs just the tests inside the current file.
- `nnoremap <leader>a :w<cr>:!make test<cr>` - This runs all unit tests.
- `nnoremap <leader>b :w<cr>:!make<cr>` - This runs a full build, useful to check for syntax errors.
