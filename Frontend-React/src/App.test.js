import { render, screen, fireEvent } from '@testing-library/react'
import App from './App'
import axios from 'axios';

afterEach(() => { jest.restoreAllMocks(); });

test('renders the footer text', () => {
    render(<App />);
    const footerElement = screen.getByText(/clearpoint.digital/i);
    expect(footerElement).toBeInTheDocument();
})

test('adding should render all active tasks', async () => {
    const mAxiosResponse = {
        data: [{ id: '123', description: "test1" }, { id: '456', description: "test2" }],
    };

    jest.spyOn(axios, 'post').mockResolvedValueOnce();
    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    var addButton = screen.getByRole('button', { name: 'Add Item' });
    fireEvent.click(addButton);

    expect(await screen.findByText('test1')).toBeInTheDocument();
    expect(await screen.findByText('test2')).toBeInTheDocument();
});


test('adding  should be able to surface errors', async () => {
    const mAxiosResponse = {
        data: [],
    };
    const mError = {
        response: { data: 'whoops, new exception' },
    };

    jest.spyOn(axios, 'post').mockRejectedValueOnce(mError);
    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    var addButton = screen.getByRole('button', { name: 'Add Item' });
    fireEvent.click(addButton);

    expect(await screen.findByText('whoops, new exception')).toBeInTheDocument();
});

test('succesfull add should clear exceptions', async () => {
    const mAxiosResponse = {
        data: [],
    };
    const mError = {
        response: { data: 'whoops, new exception' },
    };

    jest.spyOn(axios, 'post').mockRejectedValueOnce(mError).mockResolvedValueOnce();
    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    const addButton = await screen.findByRole('button', { name: 'Add Item' });
    fireEvent.click(addButton);
    const alert = await screen.findByText('whoops, new exception');

    expect(alert).toBeInTheDocument();
    fireEvent.click(addButton);
    expect(alert).not.toBeInTheDocument();
});

test('clear button should clear exceptions', async () => {
    const mAxiosResponse = {
        data: [],
    };
    const mError = {
        response: { data: 'whoops, new exception' },
    };

    jest.spyOn(axios, 'post').mockRejectedValueOnce(mError);
    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    const addButton = await screen.findByRole('button', { name: 'Add Item' });

    fireEvent.click(addButton);
    const alert = await screen.findByText('whoops, new exception');
    expect(alert).toBeInTheDocument();

    const clearButton = await screen.findByRole('button', { name: 'Clear' });
    fireEvent.click(clearButton);
    expect(alert).not.toBeInTheDocument();

});

test('component should render all active tasks on load', async () => {
    const mAxiosResponse = {
        data: [{ id: '123', description: "test1" }, { id: '456', description: "test2" }],
    };

    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    expect(await screen.findByText('test1')).toBeInTheDocument();
    expect(await screen.findByText('test2')).toBeInTheDocument();

});


test('marking as complete should clear error', async () => {
    const mAxiosResponse = {
        data: [{ id: '123', description: "test1" }, { id: '456', description: "test2" }],
    };
    const mError = {
        response: { data: 'whoops, new exception' },
    };

    jest.spyOn(axios, 'post').mockRejectedValueOnce(mError)
    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    const addButton = await screen.findByRole('button', { name: 'Add Item' });
    fireEvent.click(addButton);
    const alert = await screen.findByText('whoops, new exception');

    expect(alert).toBeInTheDocument();

    const markAsCompleteButton = await screen.findAllByRole('button', { name: 'Mark as completed' });
    fireEvent.click(markAsCompleteButton[0]);
    expect(alert).not.toBeInTheDocument();
});

test('clicking refresh should reload all items', async () => {
    const mAxiosResponse = {
        data: [{ id: '123', description: "test1" }, { id: '456', description: "test2" }],
    };

    jest.spyOn(axios, 'get').mockImplementation(() => Promise.resolve(mAxiosResponse));

    render(<App />);

    expect(await screen.findByText('test1')).toBeInTheDocument();
    expect(await screen.findByText('test2')).toBeInTheDocument();

    const refreshButton = await screen.findByRole('button', { name: 'Refresh' });

    fireEvent.click(refreshButton);
    expect(await screen.findByText('test1')).toBeInTheDocument();
    expect(await screen.findByText('test2')).toBeInTheDocument();

});